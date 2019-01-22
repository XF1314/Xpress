using System.Threading;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core.Uow 
{
    public class UnitOfWorkAccessor : IUnitOfWorkAccessor, ISingletonDependency
    {
        public IUnitOfWork UnitOfWork => _currentUow.Value;

        private readonly AsyncLocal<IUnitOfWork> _currentUow;

        public UnitOfWorkAccessor()
        {
            _currentUow = new AsyncLocal<IUnitOfWork>();
        }

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            _currentUow.Value = unitOfWork;
        }
    }
}