using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EducationalCenter.Persistence.Repositories
{
  public sealed class GradeRepository : IGradeRepository
  {
    private readonly ApplicationDbContext _context;

    public GradeRepository(ApplicationDbContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Grade?> GetByIdAsync(Guid id)
    {
      return await _context.Set<Grade>().FindAsync(id);
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
      return await _context.Set<Grade>().AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(Grade grade)
    {
      await _context.Set<Grade>().AddAsync(grade);
    }

    public void Update(Grade grade)
    {
      _context.Set<Grade>().Update(grade);
    }

    public void Delete(Grade grade)
    {
      _context.Set<Grade>().Remove(grade);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
      return await _context.Set<Grade>().AnyAsync(g => g.GradeId == id);
    }
  }
}
