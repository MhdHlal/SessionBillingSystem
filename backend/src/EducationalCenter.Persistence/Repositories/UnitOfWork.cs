using System;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Shared;
using EducationalCenter.Persistence.Contexts;

namespace EducationalCenter.Persistence.Repositories
{
  public sealed class UnitOfWork : IUnitOfWork
  {
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      return await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
