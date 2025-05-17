using System.Security.Claims;
using KaznacheystvoCourse.DTO.Answer;
using KaznacheystvoCourse.Interfaces.ISevices;
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

    [HttpGet("results/{scoreId}")]
    public async Task<IActionResult> GetTestResult(int scoreId)
    {
        try
        {
            var result = await _testService.GetTestResultAsync(scoreId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting test result {scoreId}");
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpGet("material/{materialId}/responses")]
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