using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Xpress.Core.EntityFramework;

namespace Xpress.Core.EventBus.Cap
{
    public class DbContextBasedCapTransaction : CapTransactionBase
    {
        private readonly IEfDbContextProvider _dbContextProvider;

        public DbContextBasedCapTransaction(IDispatcher dispatcher, IEfDbContextProvider dbContextProvider) : base(dispatcher)
        {
            _dbContextProvider = dbContextProvider;
        }

        public override void Commit()
        {
            DbTransaction = DbTransaction ?? _dbContextProvider.DbContextTransaction;
            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Commit();
                    break;
                case IDbContextTransaction dbContextTransaction:
                    dbContextTransaction.Commit();
                    break;
            }

            Flush();
        }

        public override void Rollback()
        {
            DbTransaction = DbTransaction ?? _dbContextProvider.DbContextTransaction;
            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Rollback();
                    break;
                case IDbContextTransaction dbContextTransaction:
                    dbContextTransaction.Rollback();
                    break;
            }
        }

        public override void Dispose()
        {
            (DbTransaction as IDbTransaction)?.Dispose();
        }

    }
}
