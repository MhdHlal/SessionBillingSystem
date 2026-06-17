using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Teachers;
using EducationalCenter.Domain.Shared;
using MediatR;

namespace EducationalCenter.Application.Teachers.Commands
{
  public sealed record UpdateTeacherCommand(Guid TeacherId, string Name) : IRequest<Unit>;

  public sealed class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, Unit>
  {
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeacherCommandHandler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
    {
      _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
      _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Unit> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
    {
      var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId);
      if (teacher == null)
        throw new KeyNotFoundException($"Teacher with ID {request.TeacherId} was not found.");

      teacher.Update(request.Name);

      _teacherRepository.Update(teacher);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}
