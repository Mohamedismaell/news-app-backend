using System.ComponentModel.DataAnnotations;

namespace NewsBackend.Application.DTOs.Comments;

public class UpdateCommentRequest
{
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}