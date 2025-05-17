using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.Course;
using KaznacheystvoCourse.Interfaces.ISevices;
using Microsoft.AspNetCore.Mvc;

namespace KaznacheystvoCourse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController:ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<CourseDto>>> GetCoursesPaginated( [FromQuery] QueryObject query)
    {
        var result = await _courseService.GetAllCoursesAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourseById(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        return course != null ? Ok(course) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateUpdateCourseDto courseDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdCourse = await _courseService.CreateCourseAsync(courseDto);
        return CreatedAtAction(nameof(GetCourseById), new { id = createdCourse.Id }, createdCourse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, CreateUpdateCourseDto courseDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _courseService.UpdateCourseAsync(id, courseDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        await _courseService.DeleteCourseAsync(id);
        return NoContent();
    }
}