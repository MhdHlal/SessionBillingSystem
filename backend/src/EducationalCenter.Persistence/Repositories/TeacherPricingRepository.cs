using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EducationalCenter.Domain.TeacherPricing;
using EducationalCenter.Persistence.Contexts;

namespace EducationalCenter.Persistence.Repositories
{
  public class TeacherPricingRepository : ITeacherPricingRepository
  {
    private readonly ApplicationDbContext _context;

    public TeacherPricingRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<TeacherGradePrice> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
      return (await _context.Set<TeacherGradePrice>()
          .Include(x => x.Teacher)
          .Include(x => x.Grade)
          .FirstOrDefaultAsync(x => x.Id == id, cancellationToken))!;
    }

    public async Task<TeacherGradePrice> GetByTeacherAndGradeAsync(Guid teacherId, Guid gradeId, CancellationToken cancellationToken = default)
    {
      return (await _context.Set<TeacherGradePrice>()
          .Include(x => x.Teacher)
          .Include(x => x.Grade)
          .FirstOrDefaultAsync(x => x.TeacherId == teacherId && x.GradeId == gradeId, cancellationToken))!;
    }

    public async Task<IEnumerable<TeacherGradePrice>> GetAllAsync(CancellationToken cancellationToken = default)
    {
      return await _context.Set<TeacherGradePrice>()
          .Include(x => x.Teacher)
          .Include(x => x.Grade)
          .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeacherGradePrice>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
      return await _context.Set<TeacherGradePrice>()
          .Include(x => x.Teacher)
          .Include(x => x.Grade)
          .Where(x => x.TeacherId == teacherId)
          .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TeacherGradePrice teacherGradePrice, CancellationToken cancellationToken = default)
    {
      await _context.Set<TeacherGradePrice>().AddAsync(teacherGradePrice, cancellationToken);
    }

    public void Update(TeacherGradePrice teacherGradePrice)
    {
      _context.Set<TeacherGradePrice>().Update(teacherGradePrice);
    }

    public void Delete(TeacherGradePrice teacherGradePrice)
    {
      _context.Set<TeacherGradePrice>().Remove(teacherGradePrice);
    }

    public async Task<bool> ExistsAsync(Guid teacherId, Guid gradeId, CancellationToken cancellationToken = default)
    {
      return await _context.Set<TeacherGradePrice>()
          .AnyAsync(x => x.TeacherId == teacherId && x.GradeId == gradeId, cancellationToken);
    }
  }
}
