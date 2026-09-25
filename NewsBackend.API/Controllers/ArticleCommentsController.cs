using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsBackend.API.Extensions;
using NewsBackend.Application.DTOs.Comments;
using NewsBackend.Application.DTOs.Common;
using NewsBackend.Application.Interfaces;

namespace NewsBackend.API.Controllers;

[ApiController]
[Route("api/v1/articles/{articleId:int}/comments")]
public class ArticleCommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public ArticleCommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<CommentResponse>>> GetByArticle(
        int articleId,
        [FromQuery] PaginationParams pagination,
        CancellationToken cancellationToken)
    {
        return Ok(await _commentService.GetByArticleAsync(articleId, pagination, cancellationToken));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CommentResponse>> Create(
        int articleId,
        [FromBody] CreateCommentRequest request,
        CancellationToken cancellationToken)
    {
        var comment = await _commentService.CreateAsync(articleId, User.GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetByArticle), new { articleId }, comment);
    }
}