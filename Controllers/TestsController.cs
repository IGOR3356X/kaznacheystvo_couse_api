using System.Security.Claims;
using KaznacheystvoCourse.DTO.Answer;
using KaznacheystvoCourse.Interfaces.ISevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KaznacheystvoCourse.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TestsController:ControllerBase
{
    private readonly ITestService _testService;
    private readonly ILogger<TestsController> _logger;

    public TestsController(ITestService testService, ILogger<TestsController> logger)
    {
        _testService = testService;
        _logger = logger;
    }

    [HttpPost("submit")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<IActionResult> SubmitTest([FromBody] SubmitTestDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid submit test model");
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _testService.SubmitTestAnswersAsync(dto.UserId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting test");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("material/GetUserAttempts/{materialId:int}")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<ActionResult<IEnumerable<UserAttemptDto>>> GetUserAttempts([FromRoute]int materialId)
    {
        try
        {
            var userId = int.Parse(User.Claims.First(claim => claim.Type == "Id").Value);
            var attempts = await _testService.GetUserAttemptsForMaterialAsync(userId, materialId);
            return Ok(attempts);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpGet("result/{scoreId}")]
    [Authorize(Roles = "Пользователь,Администратор,Модератор")]
    public async Task<ActionResult<TestResultDto>> GetTestResult(int scoreId)
    {
        try
        {
            var result = await _testService.GetTestResultAsync(scoreId);
            
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpGet("material/{materialId}/responses")]
    [Authorize(Roles = "Администратор,Модератор")]
    public async Task<IActionResult> GetAllResponsesForMaterial(int materialId)
    {
        try
        {
            var responses = await _testService.GetAllResponsesForMaterialAsync(materialId);
            return Ok(responses);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting responses for material {materialId}");
            return StatusCode(500, "Internal server error");
        }
    }
}