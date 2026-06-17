using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducationalCenter.Application.Teachers.Commands;
using EducationalCenter.Application.Teachers.Queries;
using EducationalCenter.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.API.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")] // الامتثال للقسم 19.3 و 19.4 من عقد الواجهات
  public sealed class TeachersController : ControllerBase
  {
    private readonly IMediator _mediator;

    public TeachersController(IMediator mediator)
    {
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    // 1. إنشاء معلم جديد (POST /api/v1/teachers)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherCommand command)
    {
      try
      {
        var teacherId = await _mediator.Send(command);
        var response = ApiResponse<object>.ToSuccess(new { id = teacherId });
        return CreatedAtAction(nameof(GetById), new { id = teacherId }, response);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("VALIDATION_ERROR", ex.Message);
        return BadRequest(errorResponse);
      }
    }

    // 2. جلب جميع المعلمين (GET /api/v1/teachers)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      try
      {
        var teachers = await _mediator.Send(new GetTeachersQuery());
        var response = ApiResponse<IEnumerable<TeacherDto>>.ToSuccess(teachers);
        return Ok(response);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("UNEXPECTED_SERVER_ERROR", ex.Message);
        return StatusCode(500, errorResponse);
      }
    }

    // 3. جلب معلم محدد بواسطة المعرف (GET /api/v1/teachers/{id})
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      try
      {
        var teacher = await _mediator.Send(new GetTeacherByIdQuery(id));
        if (teacher == null)
        {
          var errorResponse = ApiResponse<object>.ToError("ENTITY_NOT_FOUND", $"Teacher with ID {id} not found.");
          return NotFound(errorResponse);
        }
        var response = ApiResponse<TeacherDto>.ToSuccess(teacher);
        return Ok(response);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("UNEXPECTED_SERVER_ERROR", ex.Message);
        return StatusCode(500, errorResponse);
      }
    }

    // 4. تحديث بيانات معلم (PUT /api/v1/teachers/{id})
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherCommand command)
    {
      if (id != command.TeacherId)
      {
        var errorResponse = ApiResponse<object>.ToError("VALIDATION_ERROR", "Mismatched Teacher ID in request body and URL.");
        return BadRequest(errorResponse);
      }

      try
      {
        await _mediator.Send(command);
        var response = ApiResponse<object>.ToSuccess(new { message = "Resource updated successfully." });
        return Ok(response);
      }
      catch (KeyNotFoundException)
      {
        var errorResponse = ApiResponse<object>.ToError("ENTITY_NOT_FOUND", $"Teacher with ID {id} not found.");
        return NotFound(errorResponse);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("BUSINESS_RULE_VIOLATION", ex.Message);
        return BadRequest(errorResponse);
      }
    }

    // 5. حذف معلم (DELETE /api/v1/teachers/{id})
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      try
      {
        await _mediator.Send(new DeleteTeacherCommand(id));
        var response = ApiResponse<object>.ToSuccess(new { message = "Resource deleted successfully." });
        return Ok(response);
      }
      catch (KeyNotFoundException)
      {
        var errorResponse = ApiResponse<object>.ToError("ENTITY_NOT_FOUND", $"Teacher with ID {id} not found.");
        return NotFound(errorResponse);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("BUSINESS_RULE_VIOLATION", ex.Message);
        return BadRequest(errorResponse);
      }
    }
  }
}
