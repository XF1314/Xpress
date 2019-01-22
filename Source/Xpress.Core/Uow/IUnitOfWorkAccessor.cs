using JetBrains.Annotations;

namespace Xpress.Core.Uow
{
    public interface IUnitOfWorkAccessor
    {
        [CanBeNull]
        IUnitOfWork UnitOfWork { get; }

        void SetUnitOfWork([CanBeNull] IUnitOfWork unitOfWork);
    }
}