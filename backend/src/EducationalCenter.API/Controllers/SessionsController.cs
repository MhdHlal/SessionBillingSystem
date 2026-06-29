using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EducationalCenter.API.Models;
using EducationalCenter.Application.Sessions.Commands;
using EducationalCenter.Application.Sessions.Queries;

namespace EducationalCenter.API.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class SessionsController : ControllerBase
  {
    private readonly IMediator _mediator;

    public SessionsController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var query = new GetSessionsQuery();
      var result = await _mediator.Send(query);
      return Ok(ApiResponse<IEnumerable<SessionDto>>.ToSuccess(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      var query = new GetSessionByIdQuery(id);
      var result = await _mediator.Send(query);
      if (result == null)
      {
        return NotFound(ApiResponse<SessionDto>.ToError("NOT_FOUND", "Session not found."));
      }
      return Ok(ApiResponse<SessionDto>.ToSuccess(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSessionCommand command)
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

    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
      try
      {
        var command = new CompleteSessionCommand(id);
        await _mediator.Send(command);
        return Ok(ApiResponse<bool>.ToSuccess(true));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<bool>.ToError("BAD_REQUEST", ex.Message));
      }
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
      try
      {
        var command = new CancelSessionCommand(id);
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
