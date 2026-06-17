using System;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Domain.Shared;
using MediatR;

namespace EducationalCenter.Application.Grades.Commands
{
  public sealed record CreateGradeCommand(string Name) : IRequest<Guid>;

  public sealed class CreateGradeCommandHandler : IRequestHandler<CreateGradeCommand, Guid>
  {
    private readonly IGradeRepository _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGradeCommandHandler(IGradeRepository gradeRepository, IUnitOfWork unitOfWork)
    {
      _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
      _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Guid> Handle(CreateGradeCommand request, CancellationToken cancellationToken)
    {
      var grade = Grade.Create(request.Name);

      await _gradeRepository.AddAsync(grade);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return grade.GradeId;
    }
  }
}
