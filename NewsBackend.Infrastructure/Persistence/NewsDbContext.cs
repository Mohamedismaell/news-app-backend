using Microsoft.EntityFrameworkCore;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Persistence;

public class NewsDbContext : DbContext
{
    public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Article> Articles => Set<Article>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NewsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}