using NewsBackend.Domain.Enums;

namespace NewsBackend.Application.DTOs.Users;

public class UserResponse
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Bio { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }
}