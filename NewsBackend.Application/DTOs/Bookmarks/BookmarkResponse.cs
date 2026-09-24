using NewsBackend.Application.DTOs.Articles;

namespace NewsBackend.Application.DTOs.Bookmarks;

public class BookmarkResponse
{
    public int UserId { get; set; }

    public int ArticleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public ArticleSummaryResponse Article { get; set; } = new();
}