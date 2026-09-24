namespace NewsBackend.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public required string Token { get; set; }

    public int UserId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public User User { get; set; } = null!;
}