using NewsBackend.Application.DTOs.Users;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Services;

internal static class UserMapper
{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Username = user.Username,
            PhoneNumber = user.PhoneNumber,
            Bio = user.Bio,
            ProfilePhotoUrl = user.ProfilePhotoUrl,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public static UserSummaryResponse ToSummaryResponse(this User user)
    {
        return new UserSummaryResponse
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Username = user.Username,
            ProfilePhotoUrl = user.ProfilePhotoUrl
        };
    }
}