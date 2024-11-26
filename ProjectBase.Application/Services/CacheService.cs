using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ProjectBase.Domain.Interfaces;
using System.Collections.Concurrent;

namespace ProjectBase.Application.Services
{
    public class CacheService : ICacheService
    {
        private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();
        private readonly IDistributedCache _distributeCache;
        public CacheService(IDistributedCache distributeCache)
        {
            _distributeCache = distributeCache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancelToken = default) where T : class
        {
            string? cacheValue = await _distributeCache.GetStringAsync(key, cancelToken);
            if (cacheValue is null)
            {
                return null;
            }

            T? data = JsonConvert.DeserializeObject<T>(cacheValue);
            return data;
        }

        public async Task RemoveAsync<T>(string key, CancellationToken cancelToken = default) where T : class
        {
            await _distributeCache.RemoveAsync(key, cancelToken);

            CacheKeys.TryRemove(key, out bool _);
        }

        public async Task RemoveByPrefixKeyAsync<T>(string prefixKey, CancellationToken cancelToken = default) where T : class
        {
            IEnumerable<Task> tasks = CacheKeys
                                        .Keys
                                        .Where(x => x.StartsWith(prefixKey))
                                        .Select(async x => await RemoveAsync<T>(x, cancelToken));

            await Task.WhenAll(tasks);
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancelToken = default)
        {
            string json = JsonConvert.SerializeObject(value);

            await _distributeCache.SetStringAsync(key, json, cancelToken);

            CacheKeys.TryAdd(key, false);
        }

        public async Task UpdateNewDataInCache<T>(string key, T value, CancellationToken cancelToken = default) where T : class
        {
            await RemoveAsync<T>(key, cancelToken);
            await SetAsync<T>(key, value, cancelToken);
        }
    }
}
