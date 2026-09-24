using NewsBackend.Application.DTOs.Articles;
using NewsBackend.Application.DTOs.Common;

namespace NewsBackend.Application.Interfaces;

public interface IArticleService
{
    Task<PagedResponse<ArticleSummaryResponse>> GetArticlesAsync(ArticleQueryParams query, CancellationToken cancellationToken = default);

    Task<ArticleResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ArticleResponse> CreateAsync(int authorId, CreateArticleRequest request, CancellationToken cancellationToken = default);

    Task<ArticleResponse> UpdateAsync(int id, UpdateArticleRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<ArticleResponse> PublishAsync(int id, CancellationToken cancellationToken = default);

    Task<ArticleResponse> ArchiveAsync(int id, CancellationToken cancellationToken = default);
}