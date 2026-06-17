using EducationalCenter.Domain.Grades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationalCenter.Persistence.Configurations
{
  public sealed class GradeConfiguration : IEntityTypeConfiguration<Grade>
  {
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
      builder.ToTable("Grades");

      builder.HasKey(g => g.GradeId);

      builder.Property(g => g.Name)
          .HasMaxLength(100)
          .IsRequired();
    }
  }
}
