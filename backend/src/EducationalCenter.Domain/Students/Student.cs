using System;

namespace EducationalCenter.Domain.Students
{
  public sealed class Student
  {
    public Guid StudentId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid GradeId { get; private set; }

    private Student() { }

    public static Student Create(string name, Guid gradeId)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Student name is mandatory and cannot be empty.", nameof(name));

      if (name.Length > 200)
        throw new ArgumentException("Student name cannot exceed 200 characters.", nameof(name));

      if (gradeId == Guid.Empty)
        throw new ArgumentException("A valid GradeId is mandatory to associate the student.", nameof(gradeId));

      return new Student
      {
        StudentId = Guid.NewGuid(),
        Name = name.Trim(),
        GradeId = gradeId
      };
    }

    public void Update(string name, Guid gradeId)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Student name cannot be empty.", nameof(name));

      if (name.Length > 200)
        throw new ArgumentException("Student name cannot exceed 200 characters.", nameof(name));

      if (gradeId == Guid.Empty)
        throw new ArgumentException("A valid GradeId is required.", nameof(gradeId));

      Name = name.Trim();
      GradeId = gradeId;
    }
  }
}
