using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsBackend.API.Extensions;
using NewsBackend.Application.DTOs.Articles;
using NewsBackend.Application.DTOs.Common;
using NewsBackend.Application.Interfaces;

namespace NewsBackend.API.Controllers;

[ApiController]
[Route("api/v1/bookmarks")]
[Authorize]
public class BookmarksController : ControllerBase
{
    private readonly IBookmarkService _bookmarkService;

    public BookmarksController(IBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ArticleSummaryResponse>>> GetBookmarks(
        [FromQuery] PaginationParams pagination,
        CancellationToken cancellationToken)
    {
        return Ok(await _bookmarkService.GetBookmarkedArticlesAsync(User.GetUserId(), pagination, cancellationToken));
    }
}