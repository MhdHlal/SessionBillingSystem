using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducationalCenter.Domain.Teachers;
using EducationalCenter.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EducationalCenter.Persistence.Repositories
{
  public sealed class TeacherRepository : ITeacherRepository
  {
    private readonly ApplicationDbContext _context;

    public TeacherRepository(ApplicationDbContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Teacher?> GetByIdAsync(Guid id)
    {
      return await _context.Set<Teacher>().FindAsync(id);
    }

    public async Task<IEnumerable<Teacher>> GetAllAsync()
    {
      return await _context.Set<Teacher>().AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(Teacher teacher)
    {
      await _context.Set<Teacher>().AddAsync(teacher);
    }

    public void Update(Teacher teacher)
    {
      _context.Set<Teacher>().Update(teacher);
    }

    public void Delete(Teacher teacher)
    {
      _context.Set<Teacher>().Remove(teacher);
    }
  }
}
