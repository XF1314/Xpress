using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.EventBus.Cap
{
     public static  class CapTransactionExtensions
    {
        public static ICapTransaction Begin(this ICapTransaction transaction, bool autoCommit = false)
        {
            transaction.AutoCommit = autoCommit;

            return transaction;
        }

    }
}
