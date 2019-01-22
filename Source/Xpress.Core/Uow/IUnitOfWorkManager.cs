using JetBrains.Annotations;

namespace Xpress.Core.Uow
{
    public interface IUnitOfWorkManager
    {
        [CanBeNull]
        IUnitOfWork Current { get; }

        [NotNull]
        IUnitOfWork Begin([NotNull] IUnitOfWorkOptions unitOfWorkOptions, bool requiresNew = false);

        [NotNull]
        IUnitOfWork Reserve([NotNull] string reservationName, bool requiresNew = false);

        void BeginReserved([NotNull] string reservationName, [NotNull] IUnitOfWorkOptions unitOfWorkOptions);

        bool TryBeginReserved([NotNull] string reservationName, [NotNull] IUnitOfWorkOptions unitOfWorkOptions);
    }
}