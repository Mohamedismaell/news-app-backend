using System.ComponentModel.DataAnnotations;

namespace NewsBackend.Application.DTOs.Comments;

public class CreateCommentRequest
{
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}