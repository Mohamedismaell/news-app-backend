using Microsoft.EntityFrameworkCore;
using NewsBackend.Application.DTOs.Users;
using NewsBackend.Application.Exceptions;
using NewsBackend.Application.Interfaces;

namespace NewsBackend.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IAppDbContext _context;

    public UserService(IAppDbContext context)
    {
        _context = context;
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

    public Task<UserResponse> UpdateProfileAsync(int userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Profile update is implemented in a later phase.");
    }

    public Task<UserResponse> UpdateProfilePhotoAsync(int userId, string profilePhotoUrl, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Profile photo management is implemented in a later phase.");
    }

    public Task<UserResponse> RemoveProfilePhotoAsync(int userId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Profile photo management is implemented in a later phase.");
    }
}