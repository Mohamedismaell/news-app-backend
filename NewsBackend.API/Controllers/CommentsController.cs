using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsBackend.API.Extensions;
using NewsBackend.Application.DTOs.Comments;
using NewsBackend.Application.Interfaces;

namespace NewsBackend.API.Controllers;

[ApiController]
[Route("api/v1/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentResponse>> Update(
        int id,
        [FromBody] UpdateCommentRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _commentService.UpdateAsync(id, User.GetUserId(), request, cancellationToken));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _commentService.DeleteAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/approve")]
    public async Task<ActionResult<CommentResponse>> Approve(int id, CancellationToken cancellationToken)
    {
        return Ok(await _commentService.SetApprovalAsync(id, true, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/reject")]
    public async Task<ActionResult<CommentResponse>> Reject(int id, CancellationToken cancellationToken)
    {
        return Ok(await _commentService.SetApprovalAsync(id, false, cancellationToken));
    }
}