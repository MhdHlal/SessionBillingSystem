using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EducationalCenter.Domain.Sessions;

namespace EducationalCenter.Persistence.Configurations
{
  public class SessionConfiguration : IEntityTypeConfiguration<Session>
  {
    public void Configure(EntityTypeBuilder<Session> builder)
    {
      builder.ToTable("Sessions");

      builder.HasKey(x => x.Id);

      builder.Property(x => x.SessionDate)
          .IsRequired();

      builder.Property(x => x.UnitPrice)
          .HasPrecision(18, 2)
          .IsRequired();

      builder.Property(x => x.Status)
          .HasConversion<int>()
          .IsRequired();

      builder.Property(x => x.CreatedAt)
          .IsRequired();

      builder.Property(x => x.UpdatedAt);

      builder.HasOne(x => x.Teacher)
          .WithMany()
          .HasForeignKey(x => x.TeacherId)
          .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(x => x.Grade)
          .WithMany()
          .HasForeignKey(x => x.GradeId)
          .OnDelete(DeleteBehavior.Restrict);

      builder.HasMany(x => x.Attendances)
          .WithOne()
          .HasForeignKey(x => x.SessionId)
          .OnDelete(DeleteBehavior.Cascade);

      builder.Navigation(x => x.Attendances)
          .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
  }
}
