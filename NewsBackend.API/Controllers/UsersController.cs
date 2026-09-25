using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsBackend.API.Extensions;
using NewsBackend.Application.DTOs.Users;
using NewsBackend.Application.Exceptions;
using NewsBackend.Application.Interfaces;

namespace NewsBackend.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private readonly IUserService _userService;
    private readonly IImageService _imageService;

    public UsersController(IUserService userService, IImageService imageService)
    {
        _userService = userService;
        _imageService = imageService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetCurrentUserAsync(User.GetUserId(), cancellationToken));
    }

    [HttpPatch("me")]
    public async Task<ActionResult<UserResponse>> UpdateProfile(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _userService.UpdateProfileAsync(User.GetUserId(), request, cancellationToken));
    }

    [HttpPost("me/photo")]
    public async Task<ActionResult<UserResponse>> UploadProfilePhoto(IFormFile file, CancellationToken cancellationToken)
    {
        ValidatePhoto(file);

        await using var stream = file.OpenReadStream();
        var imageUrl = await _imageService.UploadAsync(stream, file.FileName, cancellationToken);

        return Ok(await _userService.UpdateProfilePhotoAsync(User.GetUserId(), imageUrl, cancellationToken));
    }

    [HttpDelete("me/photo")]
    public async Task<ActionResult<UserResponse>> RemoveProfilePhoto(CancellationToken cancellationToken)
    {
        return Ok(await _userService.RemoveProfilePhotoAsync(User.GetUserId(), cancellationToken));
    }

    private static void ValidatePhoto(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            throw new ValidationException("A non-empty image file is required.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new ValidationException("Image must be 5 MB or smaller.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension) || !AllowedContentTypes.Contains(file.ContentType))
        {
            throw new ValidationException("Only JPG, PNG, or WebP images are allowed.");
        }
    }
}