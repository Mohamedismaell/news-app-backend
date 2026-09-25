using System.Text.Json.Serialization;
using NewsBackend.Application.DTOs.Categories;
using NewsBackend.Application.DTOs.Users;
using NewsBackend.Domain.Enums;

namespace NewsBackend.Application.DTOs.Articles;

public class ArticleSummaryResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string? ImageUrl { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter<ArticleStatus>))]
    public ArticleStatus Status { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public CategoryResponse Category { get; set; } = new();

    public UserSummaryResponse Author { get; set; } = new();
}