using System;
using System.Data;

namespace Xpress.Core.Uow
{
    public interface IUnitOfWorkOptions
    {
        bool IsTransactional { get; set; }

        IsolationLevel IsolationLevel { get; set; }

        TimeSpan? Timeout { get; set; }

        UnitOfWorkOptions Clone();

    }
}