using System.Threading;
using System.Threading.Tasks;

namespace Xpress.Core.Uow
{
    public interface ISupportsRollback
    {
        void Rollback();

        Task RollbackAsync(CancellationToken cancellationToken);
    }
}