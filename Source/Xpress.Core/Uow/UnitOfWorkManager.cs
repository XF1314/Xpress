using Microsoft.Extensions.DependencyInjection;
using System;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Exceptions;
using Xpress.Core.Threading;
using Xpress.Core.Utils;

namespace Xpress.Core.Uow
{
    public class UnitOfWorkManager : IUnitOfWorkManager, ISingletonDependency
    {
        public IUnitOfWork Current => GetCurrentUnitOfWork();

        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWorkAccessor _unitOfWorkAccessor;
        private readonly IAsyncLocalObjectProvider _asyncLocalObjectProvider;

        public UnitOfWorkManager(IServiceProvider serviceProvider,
            IUnitOfWorkAccessor unitOfWorkAccessor, IAsyncLocalObjectProvider asyncLocalObjectProvider)
        {
            _serviceProvider = serviceProvider;
            _unitOfWorkAccessor = unitOfWorkAccessor;
            _asyncLocalObjectProvider = asyncLocalObjectProvider;
        }

        public IUnitOfWork Begin(IUnitOfWorkOptions unitOfWorkOptions, bool requiresNew = false)
        {
            Check.NotNull(unitOfWorkOptions, nameof(unitOfWorkOptions));

            var currentUow = Current;
            if (currentUow != null && !requiresNew)
                return new ChildUnitOfWork(currentUow);
            else
            {
                var unitOfWork = CreateNewUnitOfWork();
                unitOfWork.Initialize(unitOfWorkOptions);
                return unitOfWork;
            }
        }

        public IUnitOfWork Reserve(string reservationName, bool requiresNew = false)
        {
            Check.NotNull(reservationName, nameof(reservationName));

            var ambientUnitOfWork = _unitOfWorkAccessor.UnitOfWork;
            if (!requiresNew &&
                ambientUnitOfWork != null &&
                ambientUnitOfWork.IsReservedFor(reservationName))
                return new ChildUnitOfWork(ambientUnitOfWork);
            else
            {
                var unitOfWork = CreateNewUnitOfWork();
                unitOfWork.Reserve(reservationName);
                return unitOfWork;
            }
        }

        public void BeginReserved(string reservationName, IUnitOfWorkOptions unitOfWorkOptions)
        {
            if (!TryBeginReserved(reservationName, unitOfWorkOptions))
            {
                throw new XpressException($"Could not find a reserved unit of work with reservation name: {reservationName}");
            }
        }

        public bool TryBeginReserved(string reservationName, IUnitOfWorkOptions unitOfWorkOptions)
        {
            Check.NotNull(reservationName, nameof(reservationName));

            //Find reserved unit of work starting from current and going to outers
            var ambientUnitOfWork = _unitOfWorkAccessor.UnitOfWork;
            while (ambientUnitOfWork != null && !ambientUnitOfWork.IsReservedFor(reservationName))
            {
                ambientUnitOfWork = ambientUnitOfWork.Outer;
            }

            if (ambientUnitOfWork == null)
                return false;
            else
            {
                ambientUnitOfWork.Initialize(unitOfWorkOptions);
                return true;
            }
        }

        private IUnitOfWork GetCurrentUnitOfWork()
        {
            //Skip reserved unit of work
            var reserveredUow = _unitOfWorkAccessor.UnitOfWork;
            while (reserveredUow != null && (reserveredUow.IsReserved || reserveredUow.IsDisposed || reserveredUow.IsCompleted))
            {
                reserveredUow = reserveredUow.Outer;
            }

            return reserveredUow;
        }

        private IUnitOfWork CreateNewUnitOfWork()
        {
            var scope = _serviceProvider.CreateScope();
            try
            {
                var outerUow = _unitOfWorkAccessor.UnitOfWork;
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                unitOfWork.SetOuter(outerUow);
                _unitOfWorkAccessor.SetUnitOfWork(unitOfWork);
                unitOfWork.Disposed += (sender, args) =>
                {
                    _unitOfWorkAccessor.SetUnitOfWork(outerUow);
                    scope.Dispose();
                };

                return unitOfWork;
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }
    }
}