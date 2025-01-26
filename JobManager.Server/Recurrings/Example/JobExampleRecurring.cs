using Hangfire;
using JobManager.Application.Configurations;
using JobManager.Application.Handlers.Example;
using JobManager.Application.Models.Jobs.Base;
using JobManager.Application.Models.Jobs.Parameters;

namespace JobManager.Server.Recurrings.Example
{
    public class JobExampleRecurring : IJobRecurring
    {
        public async Task Start(object? parameters = null)
        {
            var defaultParameters = new JobExampleParameters() { GroupList = new() { "Group1", "Group2" } };
            BackgroundJob.Enqueue<JobExampleHandler>(handler => handler.Handle(JobRequest.Recurring(defaultParameters ?? parameters), null));
        }
    }
}
