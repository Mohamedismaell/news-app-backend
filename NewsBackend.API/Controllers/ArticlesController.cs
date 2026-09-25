using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsBackend.API.Extensions;
using NewsBackend.API.Middleware;
using NewsBackend.Application.DTOs.Articles;
using NewsBackend.Application.DTOs.Common;
using NewsBackend.Application.Interfaces;
using NewsBackend.Domain.Enums;

namespace NewsBackend.API.Controllers;

[ApiController]
[Route("api/v1/articles")]
public class ArticlesController : ControllerBase
{
    private static readonly string[] ContentRoles = ["Editor", "Admin"];

    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ArticleSummaryResponse>>> GetAll(
        [FromQuery] ArticleQueryParams query,
        CancellationToken cancellationToken)
    {
        if (query.Status.HasValue && !User.IsInRoles(ContentRoles))
        {
            return ForbiddenError();
        }

        return Ok(await _articleService.GetArticlesAsync(query, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ArticleResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var article = await _articleService.GetByIdAsync(id, cancellationToken);
        if (article.Status != ArticleStatus.Published && !User.IsInRoles(ContentRoles))
        {
            return ForbiddenError();
        }

        return Ok(article);
    }

    private ObjectResult ForbiddenError()
    {
        return new ObjectResult(new ErrorResponse(
            StatusCodes.Status403Forbidden,
            "You do not have permission to access this resource.",
            null,
            HttpContext.TraceIdentifier))
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPost]
    public async Task<ActionResult<ArticleResponse>> Create([FromBody] CreateArticleRequest request, CancellationToken cancellationToken)
    {
        var article = await _articleService.CreateAsync(User.GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ArticleResponse>> Update(int id, [FromBody] UpdateArticleRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _articleService.UpdateAsync(id, request, cancellationToken));
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _articleService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPost("{id:int}/publish")]
    public async Task<ActionResult<ArticleResponse>> Publish(int id, CancellationToken cancellationToken)
    {
        return Ok(await _articleService.PublishAsync(id, cancellationToken));
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPost("{id:int}/archive")]
    public async Task<ActionResult<ArticleResponse>> Archive(int id, CancellationToken cancellationToken)
    {
        return Ok(await _articleService.ArchiveAsync(id, cancellationToken));
    }
}