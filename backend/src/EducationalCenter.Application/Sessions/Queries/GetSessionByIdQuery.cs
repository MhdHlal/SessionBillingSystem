using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.Sessions;

namespace EducationalCenter.Application.Sessions.Queries
{
  public record GetSessionByIdQuery(Guid Id) : IRequest<SessionDto?>;

  public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, SessionDto?>
  {
    private readonly ISessionRepository _sessionRepository;

    public GetSessionByIdQueryHandler(ISessionRepository sessionRepository)
    {
      _sessionRepository = sessionRepository;
    }

    public async Task<SessionDto?> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
    {
      var session = await _sessionRepository.GetByIdAsync(request.Id, cancellationToken);
      if (session == null)
      {
        return null;
      }

      return new SessionDto(
          session.Id,
          session.TeacherId,
          session.Teacher != null ? session.Teacher.Name : string.Empty,
          session.GradeId,
          session.Grade != null ? session.Grade.Name : string.Empty,
          session.SessionDate,
          session.UnitPrice,
          session.Status.ToString(),
          session.Attendances.Select(a => new SessionAttendanceDto(
              a.Id,
              a.StudentId,
              a.Student != null ? a.Student.Name : string.Empty,
              a.IsPresent,
              a.Notes
          )).ToList()
      );
    }
  }
}
