using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EducationalCenter.Domain.Students
{
  public interface IStudentRepository
  {
    Task<Student?> GetByIdAsync(Guid id);
    Task<IEnumerable<Student>> GetAllAsync();
    Task AddAsync(Student student);
    void Update(Student student);
    void Delete(Student student);
  }
}
