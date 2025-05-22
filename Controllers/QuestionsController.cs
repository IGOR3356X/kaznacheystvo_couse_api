using KaznacheystvoCourse.DTO.Option;
using KaznacheystvoCourse.DTO.Question;
using KaznacheystvoCourse.Interfaces.ISevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KaznacheystvoCourse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions(int materialId)
    {
        var questions = await _questionService.GetQuestionsByMaterialAsync(materialId);
        return Ok(questions);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<ActionResult<QuestionDto>> GetQuestion(int materialId, int id)
    {
        var question = await _questionService.GetQuestionAsync(id);
        return question.LearnMaterialId == materialId 
            ? Ok(question) 
            : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<ActionResult<QuestionDto>> CreateQuestion([FromBody] CreateQuestionDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var question = await _questionService.CreateQuestionAsync(dto.MaterialId, dto);
            return CreatedAtAction(nameof(GetQuestion), 
                new { id = question.Id }, question);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto dto)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        try
        {
            await _questionService.UpdateQuestionAsync(id, dto); // Передаем ID из URL
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        try
        {
            await _questionService.DeleteQuestionAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}