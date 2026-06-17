using EducationalCenter.Domain.Teachers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationalCenter.Persistence.Configurations
{
  public sealed class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
  {
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
      builder.ToTable("Teachers");

      builder.HasKey(t => t.TeacherId);

      builder.Property(t => t.Name)
          .HasMaxLength(200)
          .IsRequired();
    }
  }
}
