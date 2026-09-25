using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsBackend.API.Extensions;
using NewsBackend.Application.Interfaces;

namespace NewsBackend.API.Controllers;

[ApiController]
[Route("api/v1/articles/{articleId:int}/bookmark")]
[Authorize]
public class ArticleBookmarksController : ControllerBase
{
    private readonly IBookmarkService _bookmarkService;

    public ArticleBookmarksController(IBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    [HttpPost]
    public async Task<IActionResult> Bookmark(int articleId, CancellationToken cancellationToken)
    {
        await _bookmarkService.BookmarkAsync(User.GetUserId(), articleId, cancellationToken);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveBookmark(int articleId, CancellationToken cancellationToken)
    {
        await _bookmarkService.RemoveBookmarkAsync(User.GetUserId(), articleId, cancellationToken);
        return NoContent();
    }
}