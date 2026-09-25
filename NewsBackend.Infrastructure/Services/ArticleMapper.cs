using NewsBackend.Application.DTOs.Articles;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Services;

internal static class ArticleMapper
{
    public static ArticleSummaryResponse ToSummaryResponse(this Article article)
    {
        return new ArticleSummaryResponse
        {
            Id = article.Id,
            Title = article.Title,
            Summary = article.Summary,
            ImageUrl = article.ImageUrl,
            Status = article.Status,
            PublishedAt = article.PublishedAt,
            UpdatedAt = article.UpdatedAt,
            Category = article.Category.ToResponse(),
            Author = article.Author.ToSummaryResponse()
        };
    }

    public static ArticleResponse ToResponse(this Article article)
    {
        return new ArticleResponse
        {
            Id = article.Id,
            Title = article.Title,
            Summary = article.Summary,
            ImageUrl = article.ImageUrl,
            Status = article.Status,
            PublishedAt = article.PublishedAt,
            UpdatedAt = article.UpdatedAt,
            Category = article.Category.ToResponse(),
            Author = article.Author.ToSummaryResponse(),
            Content = article.Content,
            CreatedAt = article.CreatedAt
        };
    }
}