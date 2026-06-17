using System;
using System.Collections.Generic;

namespace EducationalCenter.Domain.Grades
{
  public sealed class Grade
  {
    public Guid GradeId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    // ممارسة هندسية لمنع كسر الـ ORM أثناء الـ Materialization
    private Grade() { }

    public static Grade Create(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Grade name is mandatory and cannot be empty.", nameof(name));

      if (name.Length > 100)
        throw new ArgumentException("Grade name cannot exceed 100 characters.", nameof(name));

      return new Grade
      {
        GradeId = Guid.NewGuid(),
        Name = name.Trim()
      };
    }

    public void Update(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Grade name cannot be empty.", nameof(name));

      if (name.Length > 100)
        throw new ArgumentException("Grade name cannot exceed 100 characters.", nameof(name));

      Name = name.Trim();
    }
  }
}
