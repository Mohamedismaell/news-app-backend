namespace NewsBackend.Domain.Entities;

public class Bookmark
{
    public int UserId { get; set; }

    public int ArticleId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;

    public Article Article { get; set; } = null!;
}