using System;

namespace Xpress.Core.Domain.Entities
{
    /// <summary>
    /// Inherit from this interface must have a creation time
    /// </summary>
    public interface IHasCreationTime
    {
        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        DateTime CreationTime { get; set; }
    }
}