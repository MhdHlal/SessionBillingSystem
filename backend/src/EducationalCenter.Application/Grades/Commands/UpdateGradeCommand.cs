using System;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Domain.Shared;
using MediatR;

namespace EducationalCenter.Application.Grades.Commands
{
  public sealed record UpdateGradeCommand(Guid GradeId, string Name) : IRequest<Unit>;

  public sealed class UpdateGradeCommandHandler : IRequestHandler<UpdateGradeCommand, Unit>
  {
    private readonly IGradeRepository _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGradeCommandHandler(IGradeRepository gradeRepository, IUnitOfWork unitOfWork)
    {
      _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
      _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Unit> Handle(UpdateGradeCommand request, CancellationToken cancellationToken)
    {
      var grade = await _gradeRepository.GetByIdAsync(request.GradeId);
      if (grade == null)
        throw new KeyNotFoundException($"Grade with ID {request.GradeId} was not found.");

      grade.Update(request.Name);

      _gradeRepository.Update(grade);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}
