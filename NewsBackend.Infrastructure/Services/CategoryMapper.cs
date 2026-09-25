using NewsBackend.Application.DTOs.Categories;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Services;

internal static class CategoryMapper
{
    public static CategoryResponse ToResponse(this Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            CreatedAt = category.CreatedAt
        };
    }
}