using NewsBackend.Application.DTOs.Comments;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Services;

internal static class CommentMapper
{
    public static CommentResponse ToResponse(this Comment comment)
    {
        return new CommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            IsApproved = comment.IsApproved,
            ArticleId = comment.ArticleId,
            UserId = comment.UserId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            User = comment.User.ToSummaryResponse()
        };
    }
}