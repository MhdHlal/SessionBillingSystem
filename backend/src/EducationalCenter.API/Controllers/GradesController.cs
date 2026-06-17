using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducationalCenter.Application.Grades.Commands;
using EducationalCenter.Application.Grades.Queries;
using EducationalCenter.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.API.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")] // الامتثال للقسم 19.3 و 19.4 من عقد الواجهات
  public sealed class GradesController : ControllerBase
  {
    private readonly IMediator _mediator;

    public GradesController(IMediator mediator)
    {
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    // 1. إنشاء مرحلة جديدة (POST /api/v1/grades)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGradeCommand command)
    {
      try
      {
        var gradeId = await _mediator.Send(command);
        var response = ApiResponse<object>.ToSuccess(new { id = gradeId });
        return CreatedAtAction(nameof(GetById), new { id = gradeId }, response);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("VALIDATION_ERROR", ex.Message);
        return BadRequest(errorResponse);
      }
    }

    // 2. جلب جميع المراحل (GET /api/v1/grades)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      try
      {
        var grades = await _mediator.Send(new GetGradesQuery());
        var response = ApiResponse<IEnumerable<GradeDto>>.ToSuccess(grades);
        return Ok(response);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("UNEXPECTED_SERVER_ERROR", ex.Message);
        return StatusCode(500, errorResponse);
      }
    }

    // 3. جلب مرحلة محددة بواسطة المعرف (GET /api/v1/grades/{id})
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      try
      {
        var grade = await _mediator.Send(new GetGradeByIdQuery(id));
        if (grade == null)
        {
          var errorResponse = ApiResponse<object>.ToError("ENTITY_NOT_FOUND", $"Grade with ID {id} not found.");
          return NotFound(errorResponse);
        }
        var response = ApiResponse<GradeDto>.ToSuccess(grade);
        return Ok(response);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("UNEXPECTED_SERVER_ERROR", ex.Message);
        return StatusCode(500, errorResponse);
      }
    }

    // 4. تحديث بيانات مرحلة (PUT /api/v1/grades/{id})
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGradeCommand command)
    {
      if (id != command.GradeId)
      {
        var errorResponse = ApiResponse<object>.ToError("VALIDATION_ERROR", "Mismatched Grade ID in request body and URL.");
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
        var errorResponse = ApiResponse<object>.ToError("ENTITY_NOT_FOUND", $"Grade with ID {id} not found.");
        return NotFound(errorResponse);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("BUSINESS_RULE_VIOLATION", ex.Message);
        return BadRequest(errorResponse);
      }
    }

    // 5. حذف مرحلة (DELETE /api/v1/grades/{id})
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      try
      {
        await _mediator.Send(new DeleteGradeCommand(id));
        var response = ApiResponse<object>.ToSuccess(new { message = "Resource deleted successfully." });
        return Ok(response);
      }
      catch (KeyNotFoundException)
      {
        var errorResponse = ApiResponse<object>.ToError("ENTITY_NOT_FOUND", $"Grade with ID {id} not found.");
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
