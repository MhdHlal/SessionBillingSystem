using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Teachers;
using MediatR;

namespace EducationalCenter.Application.Teachers.Queries
{
  public sealed record GetTeacherByIdQuery(Guid TeacherId) : IRequest<TeacherDto>;

  public sealed class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, TeacherDto>
  {
    private readonly ITeacherRepository _teacherRepository;

    public GetTeacherByIdQueryHandler(ITeacherRepository teacherRepository)
    {
      _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
    }

    public async Task<TeacherDto> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
    {
      var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId);
      if (teacher == null)
        throw new KeyNotFoundException($"Teacher with ID {request.TeacherId} was not found.");

      return new TeacherDto(teacher.TeacherId, teacher.Name);
    }
  }
}
