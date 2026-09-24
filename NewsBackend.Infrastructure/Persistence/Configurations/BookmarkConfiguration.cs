using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Persistence.Configurations;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.HasKey(b => new { b.UserId, b.ArticleId });

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookmarks)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Article)
            .WithMany(a => a.Bookmarks)
            .HasForeignKey(b => b.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}