using Hangfire;
using JobManager.Application.Configurations;
using JobManager.Application.Handlers.Customer;
using JobManager.Application.Models.Jobs.Base;

namespace JobManager.Server.Recurrings.Customer
{
    public class WeeklyJob1Recurring : IJobRecurring
    {
        public async Task Start(object? parameters = null)
        {
            BackgroundJob.Enqueue<WeeklyJob1Handler>(handler => handler.Handle(JobRequest.Recurring(parameters), null));
        }
    }
}
