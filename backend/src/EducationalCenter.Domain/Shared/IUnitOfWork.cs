using System.Threading;
using System.Threading.Tasks;

namespace EducationalCenter.Domain.Shared
{
  public interface IUnitOfWork
  {
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  }
}
