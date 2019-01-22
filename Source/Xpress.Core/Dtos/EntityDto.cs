using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Dtos
{

    [Serializable]
    public abstract class EntityDto<TIdentity> : IEntityDto<TIdentity>
        where TIdentity : IIdentity
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        public TIdentity Id { get; set; }

        public override string ToString()
        {
            return $"[DTO: {GetType().Name}] Id = {Id}";
        }
    }
}
