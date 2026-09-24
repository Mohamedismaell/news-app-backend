using NewsBackend.Domain.Enums;

namespace NewsBackend.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required string DisplayName { get; set; }

    public required string Username { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Bio { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public UserRole Role { get; set; } = UserRole.User;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Article> Articles { get; set; } = [];

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<Bookmark> Bookmarks { get; set; } = [];

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}