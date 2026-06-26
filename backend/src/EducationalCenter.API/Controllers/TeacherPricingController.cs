using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EducationalCenter.API.Models;
using EducationalCenter.Application.TeacherPricing.Commands;
using EducationalCenter.Application.TeacherPricing.Queries;

namespace EducationalCenter.API.Controllers
{
  [ApiController]
  [Route("api/teacher-pricing")]
  public class TeacherPricingController : ControllerBase
  {
    private readonly IMediator _mediator;

    public TeacherPricingController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var query = new GetTeacherPricingQuery();
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<IEnumerable<TeacherPricingDto>>.ToSuccess(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      var query = new GetTeacherPricingByIdQuery(id);
      var result = await _mediator.Send(query);
      if (result == null)
      {
        return NotFound(ApiResponse<TeacherPricingDto>.ToError("NOT_FOUND", "Teacher pricing not found."));
      }
      return Ok(ApiResponse<TeacherPricingDto>.ToSuccess(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherGradePriceCommand command)
    {
      try
      {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, ApiResponse<Guid>.ToSuccess(result));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<Guid>.ToError("BAD_REQUEST", ex.Message));
      }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherGradePriceCommand command)
    {
      if (id != command.Id)
      {
        return BadRequest(ApiResponse<bool>.ToError("BAD_REQUEST", "Id mismatch."));
      }

      try
      {
        await _mediator.Send(command);
        return Ok(ApiResponse<bool>.ToSuccess(true));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<bool>.ToError("BAD_REQUEST", ex.Message));
      }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      try
      {
        var command = new DeleteTeacherGradePriceCommand(id);
        await _mediator.Send(command);
        return Ok(ApiResponse<bool>.ToSuccess(true));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<bool>.ToError("BAD_REQUEST", ex.Message));
      }
    }
  }
}
