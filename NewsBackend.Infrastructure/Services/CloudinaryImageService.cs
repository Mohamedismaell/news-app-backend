using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsBackend.Application.Interfaces;

namespace NewsBackend.Infrastructure.Services;

public class CloudinaryImageService : IImageService
{
    private readonly CloudinaryOptions _options;
    private readonly ILogger<CloudinaryImageService> _logger;
    private Cloudinary? _cloudinary;

    public CloudinaryImageService(IOptions<CloudinaryOptions> options, ILogger<CloudinaryImageService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    private Cloudinary CloudinaryClient => _cloudinary ??= CreateClient();

    private Cloudinary CreateClient()
    {
        if (string.IsNullOrWhiteSpace(_options.CloudName) ||
            string.IsNullOrWhiteSpace(_options.ApiKey) ||
            string.IsNullOrWhiteSpace(_options.ApiSecret))
        {
            _logger.LogError("Cloudinary is not configured. Set Cloudinary:CloudName, Cloudinary:ApiKey and Cloudinary:ApiSecret.");
            throw new InvalidOperationException("Cloudinary is not configured.");
        }

        return new Cloudinary(new Account(_options.CloudName, _options.ApiKey, _options.ApiSecret));
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = _options.Folder,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await CloudinaryClient.UploadAsync(uploadParams);
        if (result.Error is not null)
        {
            _logger.LogError("Cloudinary upload failed: {Error}", result.Error.Message);
            throw new InvalidOperationException("Image upload failed.");
        }

        return result.SecureUrl.AbsoluteUri;
    }

    public async Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var publicId = ExtractPublicId(imageUrl);
        if (publicId is null)
        {
            return;
        }

        var result = await CloudinaryClient.DestroyAsync(new DeletionParams(publicId));
        if (result.Error is not null)
        {
            _logger.LogWarning("Cloudinary delete failed for public id {PublicId}: {Error}", publicId, result.Error.Message);
        }
    }

    private static string? ExtractPublicId(string imageUrl)
    {
        const string uploadMarker = "/image/upload/";
        var index = imageUrl.IndexOf(uploadMarker, StringComparison.Ordinal);
        if (index < 0)
        {
            return null;
        }

        var path = imageUrl[(index + uploadMarker.Length)..];

        var firstSlash = path.IndexOf('/');
        if (firstSlash > 0 && path[0] == 'v' && path[1..firstSlash].All(char.IsDigit))
        {
            path = path[(firstSlash + 1)..];
        }

        var dot = path.LastIndexOf('.');
        if (dot > 0)
        {
            path = path[..dot];
        }

        return path;
    }
}