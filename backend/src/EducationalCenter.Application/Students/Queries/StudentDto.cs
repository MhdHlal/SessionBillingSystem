using System;

namespace EducationalCenter.Application.Students.Queries
{
  public sealed record StudentDto(Guid StudentId, string Name, Guid GradeId);
}
