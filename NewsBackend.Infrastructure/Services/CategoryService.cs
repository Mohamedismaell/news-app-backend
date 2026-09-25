using Microsoft.EntityFrameworkCore;
using NewsBackend.Application.DTOs.Categories;
using NewsBackend.Application.Exceptions;
using NewsBackend.Application.Interfaces;
using NewsBackend.Domain.Entities;

namespace NewsBackend.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IAppDbContext _context;

    public CategoryService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(c => c.ToResponse()).ToList();
    }

    public async Task<CategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Category not found.");
        }

        return category.ToResponse();
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        if (await NameExistsAsync(name, null, cancellationToken))
        {
            throw new ConflictException("A category with this name already exists.");
        }

        var category = new Category
        {
            Name = name,
            Slug = await GenerateUniqueSlugAsync(name, null, cancellationToken),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.ToResponse();
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Category not found.");
        }

        var name = request.Name.Trim();
        if (await NameExistsAsync(name, id, cancellationToken))
        {
            throw new ConflictException("A category with this name already exists.");
        }

        category.Name = name;
        category.Slug = await GenerateUniqueSlugAsync(name, id, cancellationToken);
        category.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return category.ToResponse();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Category not found.");
        }

        var hasArticles = await _context.Articles.AnyAsync(a => a.CategoryId == id, cancellationToken);
        if (hasArticles)
        {
            throw new ConflictException("Category cannot be deleted because it contains articles.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = name.ToLower();
        return await _context.Categories.AnyAsync(
            c => c.Name.ToLower() == normalized && (excludeId == null || c.Id != excludeId),
            cancellationToken);
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var baseSlug = Slugify(name);
        var slug = baseSlug;
        var counter = 1;

        while (await _context.Categories.AnyAsync(
            c => c.Slug == slug && (excludeId == null || c.Id != excludeId),
            cancellationToken))
        {
            slug = $"{baseSlug}-{counter++}";
        }

        return slug;
    }

    private static string Slugify(string name)
    {
        var builder = new System.Text.StringBuilder();

        foreach (var ch in name.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        while (builder.Length > 0 && builder[^1] == '-')
        {
            builder.Length--;
        }

        return builder.Length == 0 ? "category" : builder.ToString();
    }
}