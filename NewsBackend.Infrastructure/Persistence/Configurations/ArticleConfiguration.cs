using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Persistence.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Summary)
            .HasMaxLength(500);

        builder.Property(a => a.Content)
            .IsRequired();

        builder.Property(a => a.ImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(a => a.CategoryId);

        builder.HasIndex(a => new { a.Status, a.PublishedAt });

        builder.HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Author)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Comments)
            .WithOne(c => c.Article)
            .HasForeignKey(c => c.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Bookmarks)
            .WithOne(b => b.Article)
            .HasForeignKey(b => b.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}