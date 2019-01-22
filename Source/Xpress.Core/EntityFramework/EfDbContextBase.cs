using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.EntityFramework
{
    /// <inheritdoc />
    public abstract class EfDbContextBase : DbContext
    {
        /// <summary>
        /// Mark if DbContext has been released
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public EfDbContextBase(DbContextOptions options) : base(options)
        {

        }

        /// <inheritdoc />
        public override void Dispose()
        {
            IsDisposed = true;
            base.Dispose();
        }
    }
}
