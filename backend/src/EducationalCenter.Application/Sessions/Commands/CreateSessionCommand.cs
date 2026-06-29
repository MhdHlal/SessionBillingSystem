using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.Sessions;
using EducationalCenter.Domain.TeacherPricing;
using EducationalCenter.Domain.Shared;

namespace EducationalCenter.Application.Sessions.Commands
{
  public record AttendanceInput(Guid StudentId, bool IsPresent, string? Notes);

  public record CreateSessionCommand(
      Guid TeacherId,
      Guid GradeId,
      DateTime SessionDate,
      List<AttendanceInput> Attendances) : IRequest<Guid>;

  public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Guid>
  {
    private readonly ISessionRepository _sessionRepository;
    private readonly ITeacherPricingRepository _pricingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSessionCommandHandler(
        ISessionRepository sessionRepository,
        ITeacherPricingRepository pricingRepository,
        IUnitOfWork unitOfWork)
    {
      _sessionRepository = sessionRepository;
      _pricingRepository = pricingRepository;
      _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
      var pricing = await _pricingRepository.GetByTeacherAndGradeAsync(request.TeacherId, request.GradeId, cancellationToken);
      if (pricing == null)
      {
        throw new Exception("No pricing found for this teacher and grade.");
      }

      var session = Session.Create(request.TeacherId, request.GradeId, request.SessionDate, pricing.SessionPrice);

      foreach (var attendance in request.Attendances)
      {
        session.AddAttendance(attendance.StudentId, attendance.IsPresent, attendance.Notes);
      }

      await _sessionRepository.AddAsync(session, cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return session.Id;
    }
  }
}
