using NewsBackend.Domain.Enums;

namespace NewsBackend.Domain.Entities;

public class Article
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Summary { get; set; }

    public required string Content { get; set; }

    public string? ImageUrl { get; set; }

    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }

    public int AuthorId { get; set; }

    public Category Category { get; set; } = null!;

    public User Author { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<Bookmark> Bookmarks { get; set; } = [];
}