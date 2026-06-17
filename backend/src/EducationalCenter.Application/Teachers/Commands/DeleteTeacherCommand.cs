using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Teachers;
using EducationalCenter.Domain.Shared;
using MediatR;

namespace EducationalCenter.Application.Teachers.Commands
{
  public sealed record DeleteTeacherCommand(Guid TeacherId) : IRequest<Unit>;

  public sealed class DeleteTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand, Unit>
  {
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTeacherCommandHandler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
    {
      _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
      _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Unit> Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
    {
      var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId);
      if (teacher == null)
        throw new KeyNotFoundException($"Teacher with ID {request.TeacherId} was not found.");

      _teacherRepository.Delete(teacher);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}
