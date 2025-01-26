using Hangfire;
using JobManager.Application.Configurations;
using JobManager.Application.Handlers.Order;
using JobManager.Application.Models.Jobs.Base;

namespace JobManager.Server.Recurrings.Order
{
    public class DailyJob2Recurring : IJobRecurring
    {
        public async Task Start(object? parameters = null)
        {
            BackgroundJob.Enqueue<DailyJob2Handler>(handler => handler.Handle(JobRequest.Recurring(parameters), null));
        }
    }
}
