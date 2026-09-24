namespace NewsBackend.Domain.Entities;

public class Comment
{
    public int Id { get; set; }

    public required string Content { get; set; }

    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public bool IsApproved { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Article Article { get; set; } = null!;

    public User User { get; set; } = null!;
}