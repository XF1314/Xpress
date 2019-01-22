using System;
using System.Collections.Generic;
using System.Data;
using Xpress.Core.Application;
using Xpress.Core.Domain.Services;
using Xpress.Core.Exceptions;
using Xpress.Core.Repositories;

namespace Xpress.Core.Uow
{
    public class DefaultUnitOfWorkOptions : IUnitOfWorkOptions
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

        /// <summary>
        /// Apply uow type
        /// </summary>
        public List<Func<Type, bool>> ConventionalUowSelectors { get; set; }

        /// <inheritdoc />
        public DefaultUnitOfWorkOptions()
        {
            ConventionalUowSelectors = new List<Func<Type, bool>>
            {
                type => typeof(IRepository).IsAssignableFrom(type),
                type => typeof(IDomainService).IsAssignableFrom(type),
                type => typeof(IApplicationService).IsAssignableFrom(type)
            };
        }

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