using NewsBackend.Application.DTOs.Users;

namespace NewsBackend.Application.DTOs.Comments;

public class CommentResponse
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public bool IsApproved { get; set; }

    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public UserSummaryResponse User { get; set; } = new();
}