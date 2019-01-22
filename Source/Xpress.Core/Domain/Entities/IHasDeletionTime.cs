using System;

namespace Xpress.Core.Domain.Entities
{
    /// <summary>
    /// Inherit from this interface must contain delete time
    /// </summary>
    public interface IHasDeletionTime : ISoftDelete
    {
        /// <summary>
        /// Deletion time of this entity.
        /// </summary>
        DateTime? DeletionTime { get; set; }
    }
}