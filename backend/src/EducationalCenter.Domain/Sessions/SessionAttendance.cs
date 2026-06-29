using System;
using EducationalCenter.Domain.Students;

namespace EducationalCenter.Domain.Sessions
{
  // يمثل سجل حضور فردي لطالب داخل جلسة محددة
  public class SessionAttendance
  {
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public bool IsPresent { get; private set; }
    public string? Notes { get; private set; }
    public DateTime RecordedAt { get; private set; }

    // علاقات التنقل لقاعدة البيانات
    public Student Student { get; private set; } = null!;

    // مشيد فارغ مخصص لعمليات الـ ORM والـ EF Core
    private SessionAttendance() { }

    private SessionAttendance(Guid id, Guid sessionId, Guid studentId, bool isPresent, string? notes)
    {
      if (id == Guid.Empty)
        throw new ArgumentException("The session Attendance ID cannot be empty.", nameof(id));

      if (sessionId == Guid.Empty)
        throw new ArgumentException("The session ID cannot be empty.", nameof(sessionId));

      if (studentId == Guid.Empty)
        throw new ArgumentException("The student ID cannot be empty.", nameof(studentId));

      Id = id;
      SessionId = sessionId;
      StudentId = studentId;
      IsPresent = isPresent;
      Notes = notes?.Trim();
      RecordedAt = DateTime.UtcNow;
    }

    // دالة المصنع لإنشاء سجل حضور آمن ومحقق برمجياً
    public static SessionAttendance Create(Guid sessionId, Guid studentId, bool isPresent, string? notes = null)
    {
      return new SessionAttendance(Guid.NewGuid(), sessionId, studentId, isPresent, notes);
    }

    // تعديل حالة الحضور والملاحظات للسجل الحالي
    public void UpdateStatus(bool isPresent, string? notes)
    {
      IsPresent = isPresent;
      Notes = notes?.Trim();
      RecordedAt = DateTime.UtcNow;
    }
  }
}
