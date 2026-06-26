using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EducationalCenter.Domain.TeacherPricing;

namespace EducationalCenter.Persistence.Configurations
{
  public class TeacherGradePriceConfiguration : IEntityTypeConfiguration<TeacherGradePrice>
  {
    public void Configure(EntityTypeBuilder<TeacherGradePrice> builder)
    {
      builder.ToTable("TeacherPricing");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.SessionPrice)
          .HasPrecision(18, 2)
          .IsRequired();

      builder.Property(x => x.CreatedAt)
          .IsRequired();

      builder.Property(x => x.UpdatedAt);

      builder.HasOne(x => x.Teacher)
          .WithMany()
          .HasForeignKey(x => x.TeacherId)
          .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(x => x.Grade)
          .WithMany()
          .HasForeignKey(x => x.GradeId)
          .OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(x => new { x.TeacherId, x.GradeId })
          .IsUnique();
    }
  }
}
