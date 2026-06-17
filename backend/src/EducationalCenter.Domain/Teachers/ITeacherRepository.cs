using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EducationalCenter.Domain.Teachers
{
  public interface ITeacherRepository
  {
    Task<Teacher?> GetByIdAsync(Guid id);
    Task<IEnumerable<Teacher>> GetAllAsync();
    Task AddAsync(Teacher teacher);
    void Update(Teacher teacher);
    void Delete(Teacher teacher);
  }
}
