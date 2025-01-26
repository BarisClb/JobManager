using Hangfire;
using JobManager.Application.Configurations;
using JobManager.Application.Handlers.Order;
using JobManager.Application.Models.Jobs.Base;

namespace JobManager.Server.Recurrings.Order
{
    public class MonthlyJob1Recurring : IJobRecurring
    {
        public async Task Start(object? parameters = null)
        {
            BackgroundJob.Enqueue<MonthlyJob1Handler>(handler => handler.Handle(JobRequest.Recurring(parameters), null));
        }
    }
}
