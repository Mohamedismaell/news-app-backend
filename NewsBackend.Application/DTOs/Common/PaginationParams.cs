using System.ComponentModel.DataAnnotations;

namespace NewsBackend.Application.DTOs.Common;

public class PaginationParams
{
    private const int MaxPageSize = 100;

    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
    public int Page { get; set; } = 1;

    [Range(1, MaxPageSize, ErrorMessage = "Limit must be between 1 and 100.")]
    public int Limit { get; set; } = 10;
}