using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducationalCenter.Application.Students.Commands;
using EducationalCenter.Application.Students.Queries;
using EducationalCenter.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.API.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")] // الامتثال الصارم للقسم 19.3 و 19.7 من عقد الواجهات
  public sealed class StudentsController : ControllerBase
  {
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    // 1. إنشاء طالب جديد (POST /api/v1/students)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentCommand command)
    {
      try
      {
        var studentId = await _mediator.Send(command);
        var response = ApiResponse<object>.ToSuccess(new { id = studentId });
        return CreatedAtAction(nameof(GetById), new { id = studentId }, response);
      }
      catch (Exception ex)
      {
        // الامتثال لمعايير التحقق من المدخلات والأخطاء (القسم 19.17)
        var errorResponse = ApiResponse<object>.ToError("VALIDATION_ERROR", ex.Message);
        return BadRequest(errorResponse);
      }
    }

    // 2. جلب جميع الطلاب (GET /api/v1/students)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      try
      {
        var students = await _mediator.Send(new GetStudentsQuery());
        var response = ApiResponse<IEnumerable<StudentDto>>.ToSuccess(students);
        return Ok(response);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("UNEXPECTED_SERVER_ERROR", ex.Message);
        return StatusCode(500, errorResponse);
      }
    }

    // 3. جلب طالب محدد بواسطة المعرف (GET /api/v1/students/{id})
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      try
      {
        var student = await _mediator.Send(new GetStudentByIdQuery(id));
        if (student == null)
        {
          var errorResponse = ApiResponse<object>.ToError("ENTITY_NOT_FOUND", $"Student with ID {id} not found.");
          return NotFound(errorResponse);
        }
        var response = ApiResponse<StudentDto>.ToSuccess(student);
        return Ok(response);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("UNEXPECTED_SERVER_ERROR", ex.Message);
        return StatusCode(500, errorResponse);
      }
    }

    // 4. تحديث بيانات طالب (PUT /api/v1/students/{id})
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentCommand command)
    {
      if (id != command.StudentId)
      {
        var errorResponse = ApiResponse<object>.ToError("VALIDATION_ERROR", "Mismatched Student ID in request body and URL.");
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
        var errorResponse = ApiResponse<object>.ToError("ENTITY_NOT_FOUND", $"Student with ID {id} not found.");
        return NotFound(errorResponse);
      }
      catch (Exception ex)
      {
        var errorResponse = ApiResponse<object>.ToError("BUSINESS_RULE_VIOLATION", ex.Message);
        return BadRequest(errorResponse);
      }
    }

    // 5. حذف طالب من النظام (DELETE /api/v1/students/{id})
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      try
      {
        await _mediator.Send(new DeleteStudentCommand(id));
        var response = ApiResponse<object>.ToSuccess(new { message = "Resource deleted successfully." });
        return Ok(response);
      }
      catch (KeyNotFoundException)
      {
        var errorResponse = ApiResponse<object>.ToError("ENTITY_NOT_FOUND", $"Student with ID {id} not found.");
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
