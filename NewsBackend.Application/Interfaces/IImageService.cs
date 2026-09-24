namespace NewsBackend.Application.Interfaces;

public interface IImageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);

    Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default);
}