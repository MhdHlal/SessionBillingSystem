using EducationalCenter.Domain.Students;
using EducationalCenter.Domain.Grades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationalCenter.Persistence.Configurations
{
  public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
  {
    public void Configure(EntityTypeBuilder<Student> builder)
    {
      builder.ToTable("Students");

      builder.HasKey(s => s.StudentId);

      builder.Property(s => s.Name)
          .HasMaxLength(200)
          .IsRequired();

      // ربط علاقة المفتاح الخارجي مع جدول المراحل الدراسية لضمان التكامل المرجعي
      builder.HasOne<Grade>()
          .WithMany()
          .HasForeignKey(s => s.GradeId)
          .OnDelete(DeleteBehavior.Restrict); // منع حذف المرحلة إذا كان بها طلاب نشطون لحماية البيانات المالية
    }
  }
}
