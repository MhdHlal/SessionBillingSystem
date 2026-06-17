using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Students;
using MediatR;

namespace EducationalCenter.Application.Students.Queries
{
  public sealed record GetStudentsQuery() : IRequest<IEnumerable<StudentDto>>;

  public sealed class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, IEnumerable<StudentDto>>
  {
    private readonly IStudentRepository _studentRepository;

    public GetStudentsQueryHandler(IStudentRepository studentRepository)
    {
      _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
    }

    public async Task<IEnumerable<StudentDto>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
      var students = await _studentRepository.GetAllAsync();
      return students.Select(s => new StudentDto(s.StudentId, s.Name, s.GradeId));
    }
  }
}
