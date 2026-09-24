using Microsoft.EntityFrameworkCore;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }

    DbSet<Article> Articles { get; }

    DbSet<Category> Categories { get; }

    DbSet<Comment> Comments { get; }

    DbSet<Bookmark> Bookmarks { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}