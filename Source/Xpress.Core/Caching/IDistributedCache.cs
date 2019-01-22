using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xpress.Core.Caching
{
    public interface IDistributedCache<TCacheItem>
        where TCacheItem : class
    {
        TCacheItem Get(string key);

        Task<TCacheItem> GetAsync([NotNull] string key, CancellationToken token = default(CancellationToken));

        TCacheItem GetOrAdd(string key, Func<TCacheItem> factory, Func<DistributedCacheEntryOptions> optionsFactory = null);

        Task<TCacheItem> GetOrAddAsync(
            [NotNull] string key,
            Func<Task<TCacheItem>> factory,
            Func<DistributedCacheEntryOptions> optionsFactory = null,
            CancellationToken token = default(CancellationToken)
        );

        void Set(string key, TCacheItem value, DistributedCacheEntryOptions options = null);

        Task SetAsync(
            [NotNull] string key,
            [NotNull] TCacheItem value,
            [CanBeNull] DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken)
        );

        void Refresh(string key);

        Task RefreshAsync(string key, CancellationToken token = default(CancellationToken));

        void Remove(string key);

        Task RemoveAsync(string key, CancellationToken token = default(CancellationToken));
    }
}
