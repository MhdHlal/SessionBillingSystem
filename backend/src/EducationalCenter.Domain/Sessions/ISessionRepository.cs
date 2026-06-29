using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EducationalCenter.Domain.Sessions
{
  // واجهة مستودع البيانات الخاصة بإدارة الجلسات والحضور لتطبيق مبادئ الـ DDD والعزل
  public interface ISessionRepository
  {
    // جلب جلسة محددة بكافة تفاصيل الحضور ومعلومات الطلاب والمعلمين الملحقة بها
    Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // جلب قائمة بكافة الجلسات المسجلة في النظام
    Task<IEnumerable<Session>> GetAllAsync(CancellationToken cancellationToken = default);

    // جلب الجلسات الخاصة بمعلم معين
    Task<IEnumerable<Session>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);

    // جلب الجلسات المسجلة لمرحلة دراسية معينة
    Task<IEnumerable<Session>> GetByGradeIdAsync(Guid gradeId, CancellationToken cancellationToken = default);

    // إضافة جلسة جديدة محلياً في الـ Context
    Task AddAsync(Session session, CancellationToken cancellationToken = default);

    // تحديث بيانات الجلسة الحالية
    void Update(Session session);

    // حذف الجلسة من النظام
    void Delete(Session session);
  }
}
