using Hangfire;
using JobManager.Application.Configurations;
using JobManager.Application.Handlers.Order;
using JobManager.Application.Models.Jobs.Base;

namespace JobManager.Server.Recurrings.Order
{
    public class HourlyJob3Recurring : IJobRecurring
    {
        public async Task Start(object? parameters = null)
        {
            BackgroundJob.Enqueue<HourlyJob3Handler>(handler => handler.Handle(JobRequest.Recurring(parameters), null));
        }
    }
}
