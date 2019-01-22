using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle.Scoped;
using System;

namespace Xpress.CastleWindsor
{
    /// <summary>
    /// Default lifetimescope within the scope of a secure thread
    /// </summary>
    public class ThreadSafeDefaultLifetimeScope : ILifetimeScope
    {
        private static readonly Action<Burden> _emptyOnAfterCreated = delegate { };
        private readonly object _syncLock = new object();
        private readonly Action<Burden> _onAfterCreated;
        private IScopeCache _scopeCache;

        /// <inheritdoc />
        public ThreadSafeDefaultLifetimeScope(IScopeCache scopeCache = null, Action<Burden> onAfterCreated = null)
        {
            this._scopeCache = scopeCache ?? new ScopeCache();
            this._onAfterCreated = onAfterCreated ?? _emptyOnAfterCreated;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            lock (_syncLock)
            {
                if (_scopeCache == null) return;
                if (_scopeCache is IDisposable disposableCache)
                {
                    disposableCache.Dispose();
                }
                _scopeCache = null;
            }
        }

        /// <inheritdoc />
        public Burden GetCachedInstance(ComponentModel model, ScopedInstanceActivationCallback createInstance)
        {
            lock (_syncLock)
            {
                var burden = _scopeCache[model];
                if (burden == null)
                {
                    _scopeCache[model] = burden = createInstance(_onAfterCreated);
                }
                return burden;
            }
        }
    }
}
