using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Xpress.Core.Extensions;
using Xpress.Core.Utils;

namespace Xpress.Core.Uow
{
    internal class ChildUnitOfWork : IUnitOfWork
    {
        public Guid Id => _parent.Id;

        public IUnitOfWorkOptions UnitOfWorkOptions => _parent.UnitOfWorkOptions;

        public IUnitOfWork Outer => _parent.Outer;

        public bool IsReserved => _parent.IsReserved;

        public bool IsDisposed => _parent.IsDisposed;

        public bool IsCompleted => _parent.IsCompleted;

        public string ReservationName => _parent.ReservationName;

        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;
        public event EventHandler<UnitOfWorkEventArgs> Disposed;

        private readonly IUnitOfWork _parent;

        public ChildUnitOfWork([NotNull] IUnitOfWork parent)
        {
            Check.NotNull(parent, nameof(parent));

            _parent = parent;

            _parent.Failed += (sender, args) => { Failed.InvokeSafely(sender, args); };
            _parent.Disposed += (sender, args) => { Disposed.InvokeSafely(sender, args); };
        }

        public void SetOuter(IUnitOfWork outer)
        {
            _parent.SetOuter(outer);
        }

        public void Initialize(IUnitOfWorkOptions unitOfWorkOptions)
        {
            _parent.Initialize(unitOfWorkOptions);
        }

        public void Reserve(string reservationName)
        {
            _parent.Reserve(reservationName);
        }

        public void SaveChanges()
        {
            _parent.SaveChanges();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _parent.SaveChangesAsync(cancellationToken);
        }

        public void Complete()
        {

        }

        public Task CompleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public void Rollback()
        {
            _parent.Rollback();
        }

        public void OnCompleted(Func<Task> handler)
        {
            _parent.OnCompleted(handler);
        }

        public void Dispose()
        {

        }

        public override string ToString()
        {
            return $"[UnitOfWork {Id}]";
        }
    }
}