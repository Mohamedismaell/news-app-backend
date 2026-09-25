using Microsoft.EntityFrameworkCore;
using NewsBackend.Application.DTOs.Users;
using NewsBackend.Application.Exceptions;
using NewsBackend.Application.Interfaces;

namespace NewsBackend.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IAppDbContext _context;
    private readonly IImageService _imageService;

    public UserService(IAppDbContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    public async Task<UserResponse> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        return user.ToResponse();
    }

    public async Task<UserResponse> UpdateProfileAsync(int userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.DisplayName))
        {
            user.DisplayName = request.DisplayName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            var username = request.Username.Trim();
            var usernameTaken = await _context.Users
                .AnyAsync(u => u.Username == username && u.Id != userId, cancellationToken);
            if (usernameTaken)
            {
                throw new ConflictException("Username is already taken.");
            }

            user.Username = username;
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            user.PhoneNumber = request.PhoneNumber.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Bio))
        {
            user.Bio = request.Bio.Trim();
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }

    public async Task<UserResponse> UpdateProfilePhotoAsync(int userId, string profilePhotoUrl, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!string.IsNullOrWhiteSpace(user.ProfilePhotoUrl))
        {
            await _imageService.DeleteAsync(user.ProfilePhotoUrl, cancellationToken);
        }

        user.ProfilePhotoUrl = profilePhotoUrl;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }

    public async Task<UserResponse> RemoveProfilePhotoAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!string.IsNullOrWhiteSpace(user.ProfilePhotoUrl))
        {
            await _imageService.DeleteAsync(user.ProfilePhotoUrl, cancellationToken);
            user.ProfilePhotoUrl = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return user.ToResponse();
    }
}