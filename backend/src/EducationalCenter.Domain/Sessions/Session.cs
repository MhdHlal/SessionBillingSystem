using System;
using System.Collections.Generic;
using System.Linq;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Domain.Teachers;

namespace EducationalCenter.Domain.Sessions
{
  // يمثل هذا الكيان حالة الجلسة التعليمية (Aggregate Root)
  public class Session
  {
    private readonly List<SessionAttendance> _attendances = new();

    public Guid Id { get; private set; }
    public Guid TeacherId { get; private set; }
    public Guid GradeId { get; private set; }
    public DateTime SessionDate { get; private set; }
    public decimal UnitPrice { get; private set; } // السعر الفردي المعتمد للحصة وقت إنشائها
    public SessionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // علاقات التنقل الخاصة بقاعدة البيانات
    public Teacher Teacher { get; private set; } = null!;
    public Grade Grade { get; private set; } = null!;
    public IReadOnlyCollection<SessionAttendance> Attendances => _attendances.AsReadOnly();

    // مشيد فارغ مخصص لعمليات الـ ORM
    private Session() { }

    private Session(Guid id, Guid teacherId, Guid gradeId, DateTime sessionDate, decimal unitPrice)
    {
      if (id == Guid.Empty)
        throw new ArgumentException("The session ID cannot be empty.", nameof(id));

      if (teacherId == Guid.Empty)
        throw new ArgumentException("The teacher ID cannot be empty.", nameof(teacherId));

      if (gradeId == Guid.Empty)
        throw new ArgumentException("The grade ID cannot be empty.", nameof(gradeId));

      if (unitPrice < 0)
        throw new ArgumentException("The unit price cannot be negative.", nameof(unitPrice));

      Id = id;
      TeacherId = teacherId;
      GradeId = gradeId;
      SessionDate = sessionDate;
      UnitPrice = unitPrice;
      Status = SessionStatus.Scheduled; // الحالة الافتراضية للجلسة عند الإنشاء
      CreatedAt = DateTime.UtcNow;
    }

    // دالة المصنع لإنشاء جلسة جديدة بشكل آمن
    public static Session Create(Guid teacherId, Guid gradeId, DateTime sessionDate, decimal unitPrice)
    {
      return new Session(Guid.NewGuid(), teacherId, gradeId, sessionDate, unitPrice);
    }

    // إضافة سجل حضور لطالب محدد داخل هذه الجلسة
    public void AddAttendance(Guid studentId, bool isPresent, string? notes = null)
    {
      if (Status != SessionStatus.Scheduled)
        throw new InvalidOperationException("The attendance sheet for an unscheduled session cannot be modified.");

      if (_attendances.Any(a => a.StudentId == studentId))
        throw new InvalidOperationException("This student is already registered in the attendance sheet for this session.");

      var attendance = SessionAttendance.Create(Id, studentId, isPresent, notes);
      _attendances.Add(attendance);
      UpdatedAt = DateTime.UtcNow;
    }

    // تحديث كشف الحضور بالكامل لجسم الجلسة
    public void UpdateAttendanceList(IEnumerable<(Guid StudentId, bool IsPresent, string? Notes)> attendanceData)
    {
      if (Status != SessionStatus.Scheduled)
        throw new InvalidOperationException("Attendance cannot be modified for a session that has ended or been cancelled.");

      _attendances.Clear();

      foreach (var item in attendanceData)
      {
        AddAttendance(item.StudentId, item.IsPresent, item.Notes);
      }

      UpdatedAt = DateTime.UtcNow;
    }

    // إنهاء الجلسة وتأكيدها محاسبياً وتحويلها لحالة مكتملة
    public void Complete()
    {
      if (Status == SessionStatus.Cancelled)
        throw new InvalidOperationException("A session that has been previously cancelled cannot be completed.");

      if (!_attendances.Any())
        throw new InvalidOperationException("A session cannot be completed without any attendance records.");

      Status = SessionStatus.Completed;
      UpdatedAt = DateTime.UtcNow;
    }

    // إلغاء الجلسة بالكامل وإيقاف فواتيرها
    public void Cancel()
    {
      if (Status == SessionStatus.Completed)
        throw new InvalidOperationException("A session that has been completed and billed in advance cannot be cancelled.");

      Status = SessionStatus.Cancelled;
      UpdatedAt = DateTime.UtcNow;
    }
  }

  // الحالات التشغيلية المعتمدة للجلسة
  public enum SessionStatus
  {
    Scheduled = 1, // مجدولة وتحت التحضير
    Completed = 2, // اكتملت وتمت فوترتها بنجاح
    Cancelled = 3  // ملغاة
  }
}
