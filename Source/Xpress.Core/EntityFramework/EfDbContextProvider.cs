using Microsoft.EntityFrameworkCore.Storage;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Threading;

namespace Xpress.Core.EntityFramework
{
    /// <inheritdoc />
    public class EfDbContextProvider : IEfDbContextProvider
    {
        private class LocalDbContextWapper
        {
            public EfDbContextBase DbContext { get; set; }

            public IDbContextTransaction DbContextTransaction { get; set; }
        }

        private readonly IAsyncLocalObjectProvider _asyncLocalObjectProvider;
        private readonly IIocResolver _iocResolver;

        /// <inheritdoc />
        public EfDbContextProvider(IAsyncLocalObjectProvider asyncLocalObjectProvider, IIocResolver iocResolver)
        {
            _asyncLocalObjectProvider = asyncLocalObjectProvider;
            _iocResolver = iocResolver;
        }

        /// <inheritdoc />
        public EfDbContextBase GetDbContext()
        {
            var localDbContext = _asyncLocalObjectProvider.GetCurrent<LocalDbContextWapper>();
            if (localDbContext == null || localDbContext.DbContext == null || localDbContext.DbContext.IsDisposed)
            {
                localDbContext = new LocalDbContextWapper()
                {
                    DbContext = _iocResolver.Resolve<EfDbContextBase>()
                };
                _asyncLocalObjectProvider.SetCurrent(localDbContext);
            }
            return localDbContext.DbContext;
        }

        /// <inheritdoc />
        public IDbContextTransaction DbContextTransaction
        {
            get => _asyncLocalObjectProvider.GetCurrent<LocalDbContextWapper>()?.DbContextTransaction;
            set => _asyncLocalObjectProvider.GetCurrent<LocalDbContextWapper>().DbContextTransaction = value;
        }
    }
}

