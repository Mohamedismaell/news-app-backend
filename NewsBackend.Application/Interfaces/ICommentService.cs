using NewsBackend.Application.DTOs.Comments;
using NewsBackend.Application.DTOs.Common;

namespace NewsBackend.Application.Interfaces;

public interface ICommentService
{
    Task<PagedResponse<CommentResponse>> GetByArticleAsync(int articleId, PaginationParams pagination, CancellationToken cancellationToken = default);

    Task<CommentResponse> CreateAsync(int articleId, int userId, CreateCommentRequest request, CancellationToken cancellationToken = default);

    Task<CommentResponse> UpdateAsync(int id, int userId, UpdateCommentRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);

    Task<CommentResponse> SetApprovalAsync(int id, bool isApproved, CancellationToken cancellationToken = default);
}