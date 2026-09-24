using NewsBackend.Application.DTOs.Articles;
using NewsBackend.Application.DTOs.Common;

namespace NewsBackend.Application.Interfaces;

public interface IBookmarkService
{
    Task<PagedResponse<ArticleSummaryResponse>> GetBookmarkedArticlesAsync(int userId, PaginationParams pagination, CancellationToken cancellationToken = default);

    Task BookmarkAsync(int userId, int articleId, CancellationToken cancellationToken = default);

    Task RemoveBookmarkAsync(int userId, int articleId, CancellationToken cancellationToken = default);
}