using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Grades;
using MediatR;

namespace EducationalCenter.Application.Grades.Queries
{
  public sealed record GetGradeByIdQuery(Guid GradeId) : IRequest<GradeDto>;

  public sealed class GetGradeByIdQueryHandler : IRequestHandler<GetGradeByIdQuery, GradeDto>
  {
    private readonly IGradeRepository _gradeRepository;

    public GetGradeByIdQueryHandler(IGradeRepository gradeRepository)
    {
      _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
    }

    public async Task<GradeDto> Handle(GetGradeByIdQuery request, CancellationToken cancellationToken)
    {
      var grade = await _gradeRepository.GetByIdAsync(request.GradeId);
      if (grade == null)
        throw new KeyNotFoundException($"Grade with ID {request.GradeId} was not found.");

      return new GradeDto(grade.GradeId, grade.Name);
    }
  }
}
