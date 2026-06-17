using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Students;
using MediatR;

namespace EducationalCenter.Application.Students.Queries
{
  public sealed record GetStudentByIdQuery(Guid StudentId) : IRequest<StudentDto>;

  public sealed class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, StudentDto>
  {
    private readonly IStudentRepository _studentRepository;

    public GetStudentByIdQueryHandler(IStudentRepository studentRepository)
    {
      _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
    }

    public async Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
      var student = await _studentRepository.GetByIdAsync(request.StudentId);
      if (student == null)
        throw new KeyNotFoundException($"Student with ID {request.StudentId} was not found.");

      return new StudentDto(student.StudentId, student.Name, student.GradeId);
    }
  }
}
