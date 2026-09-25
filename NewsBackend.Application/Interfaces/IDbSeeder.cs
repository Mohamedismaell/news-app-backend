namespace NewsBackend.Application.Interfaces;

public interface IDbSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}