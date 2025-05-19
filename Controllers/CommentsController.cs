using KaznacheystvoCourse.DTO.Comment;
using KaznacheystvoCourse.Interfaces.ISevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KaznacheystvoCourse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController: ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(
        ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("{learnMaterialId}")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(int learnMaterialId)
    {
        try
        {
            var comments = await _commentService.GetCommentsForMaterialAsync(learnMaterialId);
            return Ok(comments);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto dto)
    {
        try
        {
            var comment = await _commentService.CreateCommentAsync(dto.UserId, dto);
            return CreatedAtAction(nameof(GetComments), new { learnMaterialId = dto.LearnMaterialId }, comment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{commentId}")]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        try
        {
            await _commentService.DeleteCommentAsync(commentId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}