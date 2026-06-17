using System;

namespace EducationalCenter.Domain.Teachers
{
  public sealed class Teacher
  {
    public Guid TeacherId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Teacher() { }

    public static Teacher Create(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Teacher name is mandatory and cannot be empty.", nameof(name));

      if (name.Length > 200)
        throw new ArgumentException("Teacher name cannot exceed 200 characters.", nameof(name));

      return new Teacher
      {
        TeacherId = Guid.NewGuid(),
        Name = name.Trim()
      };
    }

    public void Update(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Teacher name cannot be empty.", nameof(name));

      if (name.Length > 200)
        throw new ArgumentException("Teacher name cannot exceed 200 characters.", nameof(name));

      Name = name.Trim();
    }
  }
}
