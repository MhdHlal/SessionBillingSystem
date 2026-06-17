using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducationalCenter.Domain.Students;
using EducationalCenter.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EducationalCenter.Persistence.Repositories
{
  public sealed class StudentRepository : IStudentRepository
  {
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
      return await _context.Set<Student>().FindAsync(id);
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
      return await _context.Set<Student>().AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(Student student)
    {
      await _context.Set<Student>().AddAsync(student);
    }

    public void Update(Student student)
    {
      _context.Set<Student>().Update(student);
    }

    public void Delete(Student student)
    {
      _context.Set<Student>().Remove(student);
    }
  }
}
