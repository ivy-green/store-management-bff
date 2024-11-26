namespace ProjectBase.Domain.Interfaces
{
    public interface ICacheService
    {
        public Task<T?> GetAsync<T>(string key, CancellationToken cancelToken = default) where T : class;
        public Task SetAsync<T>(string key, T value, CancellationToken cancelToken = default);
        public Task RemoveAsync<T>(string key, CancellationToken cancelToken = default) where T : class;
        public Task RemoveByPrefixKeyAsync<T>(string prefixKey, CancellationToken cancelToken = default) where T : class;
        Task UpdateNewDataInCache<T>(string key, T value, CancellationToken cancelToken = default) where T : class;
    }
}
