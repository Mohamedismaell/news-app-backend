using System.ComponentModel.DataAnnotations;

namespace NewsBackend.Application.DTOs.Categories;

public class UpdateCategoryRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}