using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.Sessions;

namespace EducationalCenter.Application.Sessions.Queries
{
  public record GetSessionsQuery : IRequest<IEnumerable<SessionDto>>;

  public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, IEnumerable<SessionDto>>
  {
    private readonly ISessionRepository _sessionRepository;

    public GetSessionsQueryHandler(ISessionRepository sessionRepository)
    {
      _sessionRepository = sessionRepository;
    }

    public async Task<IEnumerable<SessionDto>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
      var sessions = await _sessionRepository.GetAllAsync(cancellationToken);

      return sessions.Select(x => new SessionDto(
          x.Id,
          x.TeacherId,
          x.Teacher != null ? x.Teacher.Name : string.Empty,
          x.GradeId,
          x.Grade != null ? x.Grade.Name : string.Empty,
          x.SessionDate,
          x.UnitPrice,
          x.Status.ToString(),
          x.Attendances.Select(a => new SessionAttendanceDto(
              a.Id,
              a.StudentId,
              a.Student != null ? a.Student.Name : string.Empty,
              a.IsPresent,
              a.Notes
          )).ToList()
      ));
    }
  }
}
