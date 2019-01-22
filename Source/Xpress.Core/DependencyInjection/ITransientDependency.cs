namespace Xpress.Core.DependencyInjection
{
    /// <summary>
    /// Inheriting this interface, the dependent service will return a new object each time it is requested in different scopes.
    /// </summary>
    public interface ITransientDependency
    {
    }
}

