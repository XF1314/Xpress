using Microsoft.Extensions.Caching.Distributed;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xpress.Core.Extensions;
using Xpress.Core.Serialization;
using Xpress.Core.Threading;

namespace Xpress.Core.Caching
{
    public class DistributedCache<TCacheItem> : IDistributedCache<TCacheItem>
        where TCacheItem : class
    {
        protected string CacheName { get; set; }

        protected IDistributedCache Cache { get; }

        protected ICancellationTokenProvider CancellationTokenProvider { get; }

        protected IObjectSerializer ObjectSerializer { get; }

        protected AsyncLock AsyncLock { get; } = new AsyncLock();

        public DistributedCache(
            IDistributedCache cache,
            ICancellationTokenProvider cancellationTokenProvider,
            IObjectSerializer objectSerializer)
        {
            Cache = cache;
            CancellationTokenProvider = cancellationTokenProvider;
            ObjectSerializer = objectSerializer;

            SetDefaultOptions();
        }

        public virtual TCacheItem Get(string key)
        {
            var cachedBytes = Cache.Get(NormalizeKey(key));
            if (cachedBytes == null)
            {
                return null;
            }

            return ObjectSerializer.Deserialize<TCacheItem>(cachedBytes);
        }

        public virtual async Task<TCacheItem> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var cachedBytes = await Cache.GetAsync(NormalizeKey(key), CancellationTokenProvider.FallbackToProvider(token));
            if (cachedBytes == null)
            {
                return null;
            }

            return ObjectSerializer.Deserialize<TCacheItem>(cachedBytes);
        }

        public TCacheItem GetOrAdd(
            string key,
            Func<TCacheItem> factory,
            Func<DistributedCacheEntryOptions> optionsFactory = null)
        {
            var value = Get(key);
            if (value != null)
            {
                return value;
            }

            using (AsyncLock.Lock())
            {
                value = Get(key);
                if (value != null)
                {
                    return value;
                }

                value = factory();
                Set(key, value, optionsFactory?.Invoke());
            }

            return value;
        }

        public async Task<TCacheItem> GetOrAddAsync(
            string key,
            Func<Task<TCacheItem>> factory,
            Func<DistributedCacheEntryOptions> optionsFactory = null,
            CancellationToken token = default(CancellationToken))
        {
            var value = await GetAsync(key, token);
            if (value != null)
            {
                return value;
            }

            using (await AsyncLock.LockAsync(token))
            {
                value = await GetAsync(key, token);
                if (value != null)
                {
                    return value;
                }

                value = await factory();
                await SetAsync(key, value, optionsFactory?.Invoke(), token);
            }

            return value;
        }

        public virtual void Set(string key, TCacheItem value, DistributedCacheEntryOptions options = null)
        {
            Cache.Set(
                NormalizeKey(key),
                ObjectSerializer.Serialize(value),
                options ?? new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(20) } //TODO: implement per cache item and global defaults!!!
            );
        }

        public virtual Task SetAsync(string key, TCacheItem value, DistributedCacheEntryOptions options = null, CancellationToken token = default(CancellationToken))
        {
            return Cache.SetAsync(
                NormalizeKey(key),
                ObjectSerializer.Serialize(value),
                options ?? new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(20) }, //TODO: implement per cache item and global defaults!!!
                CancellationTokenProvider.FallbackToProvider(token)
            );
        }

        public virtual void Refresh(string key)
        {
            Cache.Refresh(NormalizeKey(key));
        }

        public virtual Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Cache.RefreshAsync(NormalizeKey(key), CancellationTokenProvider.FallbackToProvider(token));
        }

        public virtual void Remove(string key)
        {
            Cache.Remove(NormalizeKey(key));
        }

        public virtual Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Cache.RemoveAsync(NormalizeKey(key), CancellationTokenProvider.FallbackToProvider(token));
        }

        protected virtual string NormalizeKey(string key)
        {
            var normalizedKey = "c:" + CacheName + ",k:" + key;

            return normalizedKey;
        }

        protected virtual void SetDefaultOptions()
        {
            //CacheName
            var cacheNameAttribute = typeof(TCacheItem)
                .GetCustomAttributes(true)
                .OfType<CacheNameAttribute>()
                .FirstOrDefault();

            CacheName = cacheNameAttribute != null ? cacheNameAttribute.Name : typeof(TCacheItem).Name;

       }
    }
}
