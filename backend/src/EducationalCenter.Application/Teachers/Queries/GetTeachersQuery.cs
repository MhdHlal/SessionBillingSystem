using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Teachers;
using MediatR;

namespace EducationalCenter.Application.Teachers.Queries
{
  public sealed record GetTeachersQuery() : IRequest<IEnumerable<TeacherDto>>;

  public sealed class GetTeachersQueryHandler : IRequestHandler<GetTeachersQuery, IEnumerable<TeacherDto>>
  {
    private readonly ITeacherRepository _teacherRepository;

    public GetTeachersQueryHandler(ITeacherRepository teacherRepository)
    {
      _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
    }

    public async Task<IEnumerable<TeacherDto>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
    {
      var teachers = await _teacherRepository.GetAllAsync();
      return teachers.Select(t => new TeacherDto(t.TeacherId, t.Name));
    }
  }
}
