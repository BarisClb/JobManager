using JobManager.Application.ServicesDb.Interfaces;

namespace JobManager.Application.ServicesDb
{
    public class RedisService : ICacheService
    {
        public async Task GetValue(string key)
        { }

        public async Task SetValue(string key, object data, TimeSpan ttl)
        { }
    }
}
