using Microsoft.EntityFrameworkCore;
using NewsBackend.Application.DTOs.Comments;
using NewsBackend.Application.DTOs.Common;
using NewsBackend.Application.Exceptions;
using NewsBackend.Application.Interfaces;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly IAppDbContext _context;

    public CommentService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<CommentResponse>> GetByArticleAsync(
        int articleId,
        PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var dbQuery = _context.Comments
            .Include(c => c.User)
            .Where(c => c.ArticleId == articleId && c.IsApproved)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await dbQuery.CountAsync(cancellationToken);

        var items = await dbQuery
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .ToListAsync(cancellationToken);

        return new PagedResponse<CommentResponse>(
            items.Select(c => c.ToResponse()).ToList(),
            pagination.Page,
            pagination.Limit,
            totalCount);
    }

    public async Task<CommentResponse> CreateAsync(int articleId, int userId, CreateCommentRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _context.Articles.AnyAsync(a => a.Id == articleId, cancellationToken))
        {
            throw new NotFoundException("Article not found.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var comment = new Comment
        {
            Content = request.Content.Trim(),
            ArticleId = articleId,
            UserId = userId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        return (await FindCommentAsync(comment.Id, cancellationToken))!.ToResponse();
    }

    public async Task<CommentResponse> UpdateAsync(int id, int userId, UpdateCommentRequest request, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (comment is null)
        {
            throw new NotFoundException("Comment not found.");
        }

        EnsureOwner(comment, userId);

        comment.Content = request.Content.Trim();
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return (await FindCommentAsync(id, cancellationToken))!.ToResponse();
    }

    public async Task DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (comment is null)
        {
            throw new NotFoundException("Comment not found.");
        }

        EnsureOwner(comment, userId);

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<CommentResponse> SetApprovalAsync(int id, bool isApproved, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (comment is null)
        {
            throw new NotFoundException("Comment not found.");
        }

        comment.IsApproved = isApproved;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return (await FindCommentAsync(id, cancellationToken))!.ToResponse();
    }

    private static void EnsureOwner(Comment comment, int userId)
    {
        if (comment.UserId != userId)
        {
            throw new ForbiddenException("You can only manage your own comments.");
        }
    }

    private Task<Comment?> FindCommentAsync(int id, CancellationToken cancellationToken)
    {
        return _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}