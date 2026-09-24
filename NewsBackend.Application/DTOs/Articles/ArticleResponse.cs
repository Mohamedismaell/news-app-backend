namespace NewsBackend.Application.DTOs.Articles;

public class ArticleResponse : ArticleSummaryResponse
{
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}