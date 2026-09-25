using Microsoft.EntityFrameworkCore;
using NewsBackend.Application.DTOs.Articles;
using NewsBackend.Application.DTOs.Common;
using NewsBackend.Application.Exceptions;
using NewsBackend.Application.Interfaces;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Services;

public class BookmarkService : IBookmarkService
{
    private readonly IAppDbContext _context;

    public BookmarkService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<ArticleSummaryResponse>> GetBookmarkedArticlesAsync(
        int userId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var dbQuery = _context.Bookmarks
            .Where(b => b.UserId == userId)
            .Include(b => b.Article)
            .ThenInclude(a => a.Category)
            .Include(b => b.Article)
            .ThenInclude(a => a.Author)
            .OrderByDescending(b => b.CreatedAt);

        var totalCount = await dbQuery.CountAsync(cancellationToken);

        var items = await dbQuery
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .ToListAsync(cancellationToken);

        return new PagedResponse<ArticleSummaryResponse>(
            items.Select(b => b.Article.ToSummaryResponse()).ToList(),
            pagination.Page,
            pagination.Limit,
            totalCount);
    }

    public async Task BookmarkAsync(int userId, int articleId, CancellationToken cancellationToken = default)
    {
        if (!await _context.Articles.AnyAsync(a => a.Id == articleId, cancellationToken))
        {
            throw new NotFoundException("Article not found.");
        }

        var existing = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == userId && b.ArticleId == articleId, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException("Article is already bookmarked.");
        }

        _context.Bookmarks.Add(new Bookmark
        {
            UserId = userId,
            ArticleId = articleId
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveBookmarkAsync(int userId, int articleId, CancellationToken cancellationToken = default)
    {
        var bookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == userId && b.ArticleId == articleId, cancellationToken);
        if (bookmark is null)
        {
            throw new NotFoundException("Bookmark not found.");
        }

        _context.Bookmarks.Remove(bookmark);
        await _context.SaveChangesAsync(cancellationToken);
    }
}