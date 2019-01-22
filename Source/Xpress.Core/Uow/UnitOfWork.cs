using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Xpress.Core.DependencyInjection;
using Xpress.Core.EntityFramework;
using Xpress.Core.Exceptions;
using Xpress.Core.Extensions;
using Xpress.Core.Threading;
using Xpress.Core.Utils;

namespace Xpress.Core.Uow
{
    public class UnitOfWork : IUnitOfWork, ITransientDependency
    {
        public Guid Id { get; } = Guid.NewGuid();

        public IUnitOfWorkOptions UnitOfWorkOptions { get; private set; }

        public IUnitOfWork Outer { get; private set; }

        public bool IsDisposed { get; private set; }

        public bool IsCompleted { get; private set; }

        public string ReservationName { get; set; }

        public bool IsReserved { get; set; }

        protected List<Func<Task>> CompletedHandlers { get; } = new List<Func<Task>>();

        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;
        public event EventHandler<UnitOfWorkEventArgs> Disposed;

        private readonly IEfDbContextProvider _dbContextProvider;
        private Exception _exception;
        private bool _isCompleting;
        private bool _isRolledback;

        public UnitOfWork(IEfDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public virtual void SetOuter(IUnitOfWork outer)
        {
            Outer = outer;
        }

        public virtual void Initialize(IUnitOfWorkOptions unitOfWorkOptions)
        {
            Check.NotNull(unitOfWorkOptions, nameof(unitOfWorkOptions));
            if (UnitOfWorkOptions != null)
                throw new XpressException("This unit of work is already initialized before!");
            else
            {
                IsReserved = false;
                UnitOfWorkOptions = unitOfWorkOptions.Clone();
                if (UnitOfWorkOptions.IsTransactional)
                {
                    if (_dbContextProvider.DbContextTransaction == null)
                    {
                        var isoLationLevel = UnitOfWorkOptions.IsolationLevel;
                        _dbContextProvider.DbContextTransaction = _dbContextProvider.GetDbContext().Database.BeginTransaction(isoLationLevel);
                    }
                    else
                    {
                        var dbTransaction = _dbContextProvider.DbContextTransaction.GetDbTransaction();
                        _dbContextProvider.GetDbContext().Database.UseTransaction(dbTransaction);
                    }
                }
            }
        }

        public virtual void Reserve(string reservationName)
        {
            Check.NotNull(reservationName, nameof(reservationName));
            ReservationName = reservationName;
            IsReserved = true;
        }

        public virtual void SaveChanges()
        {
            _dbContextProvider.GetDbContext().SaveChanges();
        }

        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _dbContextProvider.GetDbContext().SaveChangesAsync();
        }

        public virtual void Complete()
        {
            if (_isRolledback)
                return;
            else if (IsCompleted || _isCompleting)
                throw new XpressException("Complete is called before!");
            else
            {
                try
                {
                    _isCompleting = true;
                    SaveChanges();
                    _dbContextProvider.DbContextTransaction?.Commit();
                    IsCompleted = true;
                    OnCompleted();
                }
                catch (Exception ex)
                {
                    _exception = ex;
                    throw;
                }
            }
        }

        public virtual async Task CompleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isRolledback)
                return;
            else if (IsCompleted || _isCompleting)
                throw new XpressException("Complete is called before!");
            else
            {
                try
                {
                    _isCompleting = true;
                    await SaveChangesAsync(cancellationToken);
                    _dbContextProvider.DbContextTransaction?.Commit();
                    IsCompleted = true;
                    await OnCompletedAsync();
                }
                catch (Exception ex)
                {
                    _exception = ex;
                    throw;
                }
            }
        }

        public virtual void Rollback()
        {
            if (_isRolledback)
                return;
            else
            {
                _isRolledback = true;
                _dbContextProvider.DbContextTransaction?.Rollback();
            }
        }

        public void OnCompleted(Func<Task> handler)
        {
            CompletedHandlers.Add(handler);
        }

        protected virtual void OnCompleted()
        {
            CompletedHandlers.ForEach(x => AsyncHelper.RunSync(x));
        }

        protected virtual async Task OnCompletedAsync()
        {
            foreach (var handler in CompletedHandlers)
            {
                await handler.Invoke();
            }
        }
        protected virtual void OnDisposed()
        {
            Disposed.InvokeSafely(this, new UnitOfWorkEventArgs(this));
        }

        public virtual void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            if (!IsCompleted || _exception != null)
            {
                Failed.InvokeSafely(this, new UnitOfWorkFailedEventArgs(this, _exception, _isRolledback));
            }

            OnDisposed();
        }

        public override string ToString()
        {
            return $"[UnitOfWork {Id}]";
        }
    }
}