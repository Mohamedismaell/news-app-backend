using Microsoft.EntityFrameworkCore;
using NewsBackend.Application.DTOs.Articles;
using NewsBackend.Application.DTOs.Common;
using NewsBackend.Application.Exceptions;
using NewsBackend.Application.Interfaces;
using NewsBackend.Domain.Entities;
using NewsBackend.Domain.Enums;

namespace NewsBackend.Infrastructure.Services;

public class ArticleService : IArticleService
{
    private const string DefaultSortBy = "publishedAt";

    private readonly IAppDbContext _context;

    public ArticleService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<ArticleSummaryResponse>> GetArticlesAsync(
        ArticleQueryParams query,
        CancellationToken cancellationToken = default)
    {
        var dbQuery = _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Author)
            .AsQueryable();

        dbQuery = query.Status is null
            ? dbQuery.Where(a => a.Status == ArticleStatus.Published)
            : dbQuery.Where(a => a.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = $"%{query.Search.Trim()}%";
            dbQuery = dbQuery.Where(a =>
                EF.Functions.Like(a.Title, search) ||
                EF.Functions.Like(a.Summary ?? string.Empty, search) ||
                EF.Functions.Like(a.Content, search));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var categorySlug = query.Category.Trim();
            dbQuery = dbQuery.Where(a => a.Category.Slug == categorySlug);
        }

        var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? DefaultSortBy : query.SortBy.ToLowerInvariant();
        var isDescending = string.Equals(query.Order, "desc", StringComparison.OrdinalIgnoreCase);

        dbQuery = sortBy switch
        {
            "title" => isDescending
                ? dbQuery.OrderByDescending(a => a.Title)
                : dbQuery.OrderBy(a => a.Title),
            "createdAt" => isDescending
                ? dbQuery.OrderByDescending(a => a.CreatedAt)
                : dbQuery.OrderBy(a => a.CreatedAt),
            _ => isDescending
                ? dbQuery.OrderByDescending(a => a.PublishedAt ?? a.CreatedAt)
                : dbQuery.OrderBy(a => a.PublishedAt ?? a.CreatedAt)
        };

        var totalCount = await dbQuery.CountAsync(cancellationToken);

        var items = await dbQuery
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);

        return new PagedResponse<ArticleSummaryResponse>(
            items.Select(a => a.ToSummaryResponse()).ToList(),
            query.Page,
            query.Limit,
            totalCount);
    }

    public async Task<ArticleResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await FindArticleWithRelationsAsync(id, cancellationToken);
        if (article is null)
        {
            throw new NotFoundException("Article not found.");
        }

        return article.ToResponse();
    }

    public async Task<ArticleResponse> CreateAsync(int authorId, CreateArticleRequest request, CancellationToken cancellationToken = default)
    {
        var author = await _context.Users.FirstOrDefaultAsync(u => u.Id == authorId, cancellationToken);
        if (author is null)
        {
            throw new NotFoundException("Author not found.");
        }

        if (!await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken))
        {
            throw new NotFoundException("Category not found.");
        }

        var article = new Article
        {
            Title = request.Title.Trim(),
            Summary = string.IsNullOrWhiteSpace(request.Summary) ? null : request.Summary.Trim(),
            Content = request.Content,
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
            CategoryId = request.CategoryId,
            AuthorId = author.Id
        };

        _context.Articles.Add(article);
        await _context.SaveChangesAsync(cancellationToken);

        return (await FindArticleWithRelationsAsync(article.Id, cancellationToken))!.ToResponse();
    }

    public async Task<ArticleResponse> UpdateAsync(int id, UpdateArticleRequest request, CancellationToken cancellationToken = default)
    {
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (article is null)
        {
            throw new NotFoundException("Article not found.");
        }

        if (!await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken))
        {
            throw new NotFoundException("Category not found.");
        }

        article.Title = request.Title.Trim();
        article.Summary = string.IsNullOrWhiteSpace(request.Summary) ? null : request.Summary.Trim();
        article.Content = request.Content;
        article.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
        article.CategoryId = request.CategoryId;
        article.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return (await FindArticleWithRelationsAsync(id, cancellationToken))!.ToResponse();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (article is null)
        {
            throw new NotFoundException("Article not found.");
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ArticleResponse> PublishAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (article is null)
        {
            throw new NotFoundException("Article not found.");
        }

        if (article.Status == ArticleStatus.Archived)
        {
            throw new ConflictException("Archived articles cannot be published.");
        }

        if (article.Status == ArticleStatus.Draft)
        {
            article.PublishedAt = DateTime.UtcNow;
        }

        article.Status = ArticleStatus.Published;
        article.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return (await FindArticleWithRelationsAsync(id, cancellationToken))!.ToResponse();
    }

    public async Task<ArticleResponse> ArchiveAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (article is null)
        {
            throw new NotFoundException("Article not found.");
        }

        if (article.Status == ArticleStatus.Archived)
        {
            throw new ConflictException("Article is already archived.");
        }

        article.Status = ArticleStatus.Archived;
        article.PublishedAt = null;
        article.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return (await FindArticleWithRelationsAsync(id, cancellationToken))!.ToResponse();
    }

    private Task<Article?> FindArticleWithRelationsAsync(int id, CancellationToken cancellationToken)
    {
        return _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}