using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Dtos
{

    public interface IEntityDto<TIdentity>
        where TIdentity : IIdentity
    {
        TIdentity Id { get; set; }
    }
}
