namespace JobManager.Application.ServicesDb.Interfaces
{
    public interface ICacheService
    {
        Task GetValue(string key);
        Task SetValue(string key, object data, TimeSpan ttl);
    }
}
