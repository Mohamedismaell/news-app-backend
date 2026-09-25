using Microsoft.Extensions.DependencyInjection;
using NewsBackend.Application.Interfaces;
using NewsBackend.Infrastructure.Security;
using NewsBackend.Infrastructure.Services;

namespace NewsBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IImageService, CloudinaryImageService>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}