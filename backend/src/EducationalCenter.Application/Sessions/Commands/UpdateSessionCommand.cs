using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.Sessions;
using EducationalCenter.Domain.Shared;

namespace EducationalCenter.Application.Sessions.Commands
{
  public record UpdateSessionCommand(
      Guid Id,
      DateTime SessionDate,
      List<AttendanceInput> Attendances) : IRequest;

  public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand>
  {
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSessionCommandHandler(ISessionRepository sessionRepository, IUnitOfWork unitOfWork)
    {
      _sessionRepository = sessionRepository;
      _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
    {
      var session = await _sessionRepository.GetByIdAsync(request.Id, cancellationToken);
      if (session == null)
      {
        throw new Exception("Session not found.");
      }

      if (session.Status != SessionStatus.Scheduled)
      {
        throw new InvalidOperationException("Only scheduled sessions can be updated.");
      }

      bool isModified = false;

      if (session.SessionDate != request.SessionDate)
      {
        session.UpdateSessionDate(request.SessionDate);
        isModified = true;
      }

      var field = typeof(Session).GetField("_attendances", BindingFlags.NonPublic | BindingFlags.Instance);
      if (field == null)
      {
        throw new InvalidOperationException("Backing field '_attendances' not found in Session aggregate root.");
      }

      var dbAttendances = field.GetValue(session) as List<SessionAttendance>;
      if (dbAttendances == null)
      {
        dbAttendances = new List<SessionAttendance>();
        field.SetValue(session, dbAttendances);
      }

      var incomingStudentIds = request.Attendances.Select(a => a.StudentId).ToHashSet();

      int initialCount = dbAttendances.Count;
      dbAttendances.RemoveAll(a => !incomingStudentIds.Contains(a.StudentId));
      if (dbAttendances.Count != initialCount)
      {
        isModified = true;
      }

      foreach (var incoming in request.Attendances)
      {
        var existingAttendance = dbAttendances.FirstOrDefault(a => a.StudentId == incoming.StudentId);
        if (existingAttendance != null)
        {
          if (existingAttendance.IsPresent != incoming.IsPresent || existingAttendance.Notes != incoming.Notes?.Trim())
          {
            existingAttendance.UpdateStatus(incoming.IsPresent, incoming.Notes);
            isModified = true;
          }
        }
        else
        {
          var newAttendance = SessionAttendance.Create(session.Id, incoming.StudentId, incoming.IsPresent, incoming.Notes);
          dbAttendances.Add(newAttendance);
          isModified = true;
        }
      }

      if (isModified)
      {
        var updatedAtProperty = typeof(Session).GetProperty("UpdatedAt", BindingFlags.Public | BindingFlags.Instance);
        if (updatedAtProperty != null && updatedAtProperty.CanWrite)
        {
          updatedAtProperty.SetValue(session, DateTime.UtcNow);
        }
      }

      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
  }
}
