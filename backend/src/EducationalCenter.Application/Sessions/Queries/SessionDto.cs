using System;
using System.Collections.Generic;

namespace EducationalCenter.Application.Sessions.Queries
{
  public record SessionAttendanceDto(
      Guid Id,
      Guid StudentId,
      string StudentName,
      bool IsPresent,
      string? Notes
  );

  public record SessionDto(
      Guid Id,
      Guid TeacherId,
      string TeacherName,
      Guid GradeId,
      string GradeName,
      DateTime SessionDate,
      decimal UnitPrice,
      string Status,
      List<SessionAttendanceDto> Attendances
  );
}
