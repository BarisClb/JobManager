using Hangfire;

namespace JobManager.Application.Configurations
{
    [DisableConcurrentExecution(10)]
    public interface IJobRecurring
    {
        Task Start(object? parameters = null);
    }
}
