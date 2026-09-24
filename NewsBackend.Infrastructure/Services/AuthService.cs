using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsBackend.Application.DTOs.Auth;
using NewsBackend.Application.Exceptions;
using NewsBackend.Application.Interfaces;
using NewsBackend.Domain.Entities;
using NewsBackend.Domain.Enums;
using NewsBackend.Infrastructure.Security;

namespace NewsBackend.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IJwtProvider _jwtProvider;
    private readonly JwtOptions _jwtOptions;

    public AuthService(IAppDbContext context, IJwtProvider jwtProvider, IOptions<JwtOptions> jwtOptions)
    {
        _context = context;
        _jwtProvider = jwtProvider;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(u => u.Email == email, cancellationToken))
        {
            throw new ConflictException("An account with this email already exists.");
        }

        if (await _context.Users.AnyAsync(u => u.Username == request.Username.Trim(), cancellationToken))
        {
            throw new ConflictException("This username is already taken.");
        }

        var user = new User
        {
            Email = email,
            Username = request.Username.Trim(),
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("This account has been deactivated.");
        }

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

        if (storedToken is null || storedToken.RevokedAt.HasValue)
        {
            throw new UnauthorizedException("Invalid refresh token or token has been revoked.");
        }

        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedException("This refresh token has expired.");
        }

        if (storedToken.User is null || !storedToken.User.IsActive)
        {
            throw new UnauthorizedException("This account has been deactivated.");
        }

        storedToken.RevokedAt = DateTime.UtcNow;

        var (newToken, _) = _jwtProvider.GenerateRefreshToken();

        storedToken.ReplacedByToken = newToken;

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = newToken,
            UserId = storedToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays)
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = _jwtProvider.GenerateAccessToken(storedToken.User),
            RefreshToken = newToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes),
            User = storedToken.User.ToResponse()
        };
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

        if (storedToken is null || storedToken.RevokedAt.HasValue)
        {
            return;
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(User user, CancellationToken cancellationToken)
    {
        var (refreshToken, _) = _jwtProvider.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays)
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = _jwtProvider.GenerateAccessToken(user),
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes),
            User = user.ToResponse()
        };
    }
}