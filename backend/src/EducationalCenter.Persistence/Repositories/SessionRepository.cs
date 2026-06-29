using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EducationalCenter.Domain.Sessions;
using EducationalCenter.Persistence.Contexts;

namespace EducationalCenter.Persistence.Repositories
{
  public class SessionRepository : ISessionRepository
  {
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
      return await _context.Set<Session>()
          .Include(x => x.Attendances)
          .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetAllAsync(CancellationToken cancellationToken = default)
    {
      return await _context.Set<Session>()
          .Include(x => x.Attendances)
          .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
      return await _context.Set<Session>()
          .Include(x => x.Attendances)
          .Where(x => x.TeacherId == teacherId)
          .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetByGradeIdAsync(Guid gradeId, CancellationToken cancellationToken = default)
    {
      return await _context.Set<Session>()
          .Include(x => x.Attendances)
          .Where(x => x.GradeId == gradeId)
          .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Session session, CancellationToken cancellationToken = default)
    {
      await _context.Set<Session>().AddAsync(session, cancellationToken);
    }

    public void Update(Session session)
    {
      _context.Set<Session>().Update(session);
    }

    public void Delete(Session session)
    {
      _context.Set<Session>().Remove(session);
    }
  }
}
