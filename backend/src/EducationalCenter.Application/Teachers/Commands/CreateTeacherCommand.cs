using System;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Teachers;
using EducationalCenter.Domain.Shared;
using MediatR;

namespace EducationalCenter.Application.Teachers.Commands
{
  public sealed record CreateTeacherCommand(string Name) : IRequest<Guid>;

  public sealed class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, Guid>
  {
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeacherCommandHandler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
    {
      _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
      _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Guid> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
    {
      var teacher = Teacher.Create(request.Name);

      await _teacherRepository.AddAsync(teacher);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return teacher.TeacherId;
    }
  }
}
