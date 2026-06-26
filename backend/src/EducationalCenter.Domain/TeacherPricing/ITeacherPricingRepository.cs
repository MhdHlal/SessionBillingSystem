using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EducationalCenter.Domain.TeacherPricing
{
  public interface ITeacherPricingRepository
  {
    Task<TeacherGradePrice> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TeacherGradePrice> GetByTeacherAndGradeAsync(Guid teacherId, Guid gradeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TeacherGradePrice>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TeacherGradePrice>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
    Task AddAsync(TeacherGradePrice teacherGradePrice, CancellationToken cancellationToken = default);
    void Update(TeacherGradePrice teacherGradePrice);
    void Delete(TeacherGradePrice teacherGradePrice);
    Task<bool> ExistsAsync(Guid teacherId, Guid gradeId, CancellationToken cancellationToken = default);
  }
}
