using System;

namespace Xpress.Core.Domain.Entities
{
    /// <summary>
    /// Inherit from this interface must have modification time
    /// </summary>
    public interface IHasModificationTime
    {
        /// <summary>
        /// The last modified time for this entity.
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }
}