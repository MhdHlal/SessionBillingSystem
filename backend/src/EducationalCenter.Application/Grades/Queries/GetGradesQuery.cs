using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Grades;
using MediatR;

namespace EducationalCenter.Application.Grades.Queries
{
  public sealed record GetGradesQuery() : IRequest<IEnumerable<GradeDto>>;

  public sealed class GetGradesQueryHandler : IRequestHandler<GetGradesQuery, IEnumerable<GradeDto>>
  {
    private readonly IGradeRepository _gradeRepository;

    // التصحيح: مُنشئ قياسي (Constructor) صحيح بدون كلمة class
    public GetGradesQueryHandler(IGradeRepository gradeRepository)
    {
      _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
    }

    public async Task<IEnumerable<GradeDto>> Handle(GetGradesQuery request, CancellationToken cancellationToken)
    {
      var grades = await _gradeRepository.GetAllAsync();
      return grades.Select(g => new GradeDto(g.GradeId, g.Name));
    }
  }
}
