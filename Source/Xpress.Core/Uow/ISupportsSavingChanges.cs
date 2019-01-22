using System.Threading;
using System.Threading.Tasks;

namespace Xpress.Core.Uow
{
    public interface ISupportsSavingChanges
    {
        void SaveChanges();

        Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}