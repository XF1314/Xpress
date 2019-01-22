using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Xpress.Core.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }

        event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        event EventHandler<UnitOfWorkEventArgs> Disposed;

        IUnitOfWorkOptions UnitOfWorkOptions { get; }

        IUnitOfWork Outer { get; }

        void Reserve([NotNull] string reservationName);

        bool IsReserved { get; }

        string ReservationName { get; }

        void SetOuter([CanBeNull] IUnitOfWork outer);

        void Initialize([NotNull] IUnitOfWorkOptions unitOfWorkOptions);

        void SaveChanges();

        Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        void Complete();

        Task CompleteAsync(CancellationToken cancellationToken = default(CancellationToken));

        void OnCompleted(Func<Task> handler);

        bool IsCompleted { get; }

        void Rollback();

        bool IsDisposed { get; }
    }
}
