using Hangfire.Server;
using JobManager.Application.Models.Jobs.Base;

namespace JobManager.Application.Configurations
{
    public interface IJobHandler
    {
        public Task<JobResponse> Handle(JobRequest jobRequest, PerformContext? performContext = default);
    }
}
