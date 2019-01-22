using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Xpress.Core.Uow
{
    public class UnitOfWorkOptions : IUnitOfWorkOptions
    {
        /// <summary>
        /// Default: false.
        /// </summary>
        public bool IsTransactional { get; set; }


        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// If this UOW is transactional, this option indicated the isolation level of the transaction.
        /// Uses default value if not supplied.
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        public UnitOfWorkOptions Clone()
        {
            return new UnitOfWorkOptions
            {
                IsTransactional = IsTransactional,
                IsolationLevel = IsolationLevel,
                Timeout = Timeout
            };
        }
    }
}
