using Microsoft.EntityFrameworkCore.Storage;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core.EntityFramework
{
    /// <summary>
    /// DbContext provider
    /// </summary>
    public interface IEfDbContextProvider : ITransientDependency
    {
        /// <summary>
        /// Get data context operation object
        /// </summary>
        EfDbContextBase GetDbContext();

        /// <summary>
        /// DbContext transaction
        /// </summary>
        IDbContextTransaction DbContextTransaction { get; set; }
    }
}

