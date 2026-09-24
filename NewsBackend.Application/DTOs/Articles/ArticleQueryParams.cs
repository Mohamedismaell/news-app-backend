using System.ComponentModel.DataAnnotations;
using NewsBackend.Application.DTOs.Common;
using NewsBackend.Application.Validation;
using NewsBackend.Domain.Enums;

namespace NewsBackend.Application.DTOs.Articles;

public class ArticleQueryParams : PaginationParams
{
    public string? Search { get; set; }

    public string? Category { get; set; }

    [ValidEnumValue(typeof(ArticleStatus))]
    public ArticleStatus? Status { get; set; }

    [RegularExpression("^(publishedAt|createdAt|title)$", ErrorMessage = "SortBy must be publishedAt, createdAt or title.")]
    public string? SortBy { get; set; }

    [RegularExpression("^(asc|desc)$", ErrorMessage = "Order must be asc or desc.")]
    public string Order { get; set; } = "desc";
}