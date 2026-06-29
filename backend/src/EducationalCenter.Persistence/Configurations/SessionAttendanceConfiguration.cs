using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EducationalCenter.Domain.Sessions;

namespace EducationalCenter.Persistence.Configurations
{
  public class SessionAttendanceConfiguration : IEntityTypeConfiguration<SessionAttendance>
  {
    public void Configure(EntityTypeBuilder<SessionAttendance> builder)
    {
      builder.ToTable("SessionAttendances");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.IsPresent)
          .IsRequired();

      builder.Property(x => x.Notes)
          .HasMaxLength(500);

      builder.Property(x => x.RecordedAt)
          .IsRequired();

      builder.HasOne(x => x.Student)
          .WithMany()
          .HasForeignKey(x => x.StudentId)
          .OnDelete(DeleteBehavior.Restrict);
    }
  }
}
