namespace Xpress.Core.Domain.Entities
{
    /// <summary>
    /// Object activation status
    /// </summary>
    public interface IPassivable
    {
        /// <summary>
        /// Is active
        /// </summary>
        bool IsActive { get; set; }
    }
}