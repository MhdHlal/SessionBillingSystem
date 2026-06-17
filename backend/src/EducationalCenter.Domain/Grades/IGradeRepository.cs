using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EducationalCenter.Domain.Grades
{
  public interface IGradeRepository
  {
    Task<Grade?> GetByIdAsync(Guid id);
    Task<IEnumerable<Grade>> GetAllAsync();
    Task AddAsync(Grade grade);
    void Update(Grade grade);
    void Delete(Grade grade);
    Task<bool> ExistsAsync(Guid id);
  }
}
