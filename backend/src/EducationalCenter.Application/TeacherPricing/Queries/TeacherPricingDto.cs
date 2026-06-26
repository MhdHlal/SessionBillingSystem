using System;

namespace EducationalCenter.Application.TeacherPricing.Queries
{
  public record TeacherPricingDto(
      Guid Id,
      Guid TeacherId,
      string TeacherName,
      Guid GradeId,
      string GradeName,
      decimal SessionPrice
  );
}
