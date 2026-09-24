using NewsBackend.Application.DTOs.Users;

namespace NewsBackend.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default);

    Task<UserResponse> UpdateProfileAsync(int userId, UpdateUserRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse> UpdateProfilePhotoAsync(int userId, string profilePhotoUrl, CancellationToken cancellationToken = default);

    Task<UserResponse> RemoveProfilePhotoAsync(int userId, CancellationToken cancellationToken = default);
}