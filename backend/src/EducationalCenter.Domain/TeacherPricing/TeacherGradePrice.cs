using System;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Domain.Teachers;

namespace EducationalCenter.Domain.TeacherPricing
{
  public class TeacherGradePrice
  {
    public Guid Id { get; private set; }
    public Guid TeacherId { get; private set; }
    public Guid GradeId { get; private set; }
    public decimal SessionPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Teacher Teacher { get; private set; } = null!;
    public Grade Grade { get; private set; } = null!;

    private TeacherGradePrice() { }

    public TeacherGradePrice(Guid id, Guid teacherId, Guid gradeId, decimal sessionPrice)
    {
      if (id == Guid.Empty)
        throw new ArgumentException("Id cannot be empty.", nameof(id));

      if (teacherId == Guid.Empty)
        throw new ArgumentException("TeacherId cannot be empty.", nameof(teacherId));

      if (gradeId == Guid.Empty)
        throw new ArgumentException("GradeId cannot be empty.", nameof(gradeId));

      ValidatePrice(sessionPrice);

      Id = id;
      TeacherId = teacherId;
      GradeId = gradeId;
      SessionPrice = sessionPrice;
      CreatedAt = DateTime.UtcNow;
    }

    public static TeacherGradePrice Create(Guid teacherId, Guid gradeId, decimal sessionPrice)
    {
      return new TeacherGradePrice(Guid.NewGuid(), teacherId, gradeId, sessionPrice);
    }

    public void UpdatePrice(decimal newPrice)
    {
      ValidatePrice(newPrice);
      SessionPrice = newPrice;
      UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidatePrice(decimal price)
    {
      if (price <= 0)
      {
        throw new ArgumentException("Session price must be greater than zero.", nameof(price));
      }
    }
  }
}
