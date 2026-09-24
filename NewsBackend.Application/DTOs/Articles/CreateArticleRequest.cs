using System.ComponentModel.DataAnnotations;

namespace NewsBackend.Application.DTOs.Articles;

public class CreateArticleRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}