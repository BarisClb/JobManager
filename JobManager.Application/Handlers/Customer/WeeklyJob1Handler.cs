using Hangfire.Server;
using JobManager.Application.Configurations;
using JobManager.Application.Helpers.Extensions;
using JobManager.Application.Models.Exceptions;
using JobManager.Application.Models.Jobs.Base;

namespace JobManager.Application.Handlers.Customer
{
    public class WeeklyJob1Handler : IJobHandler
    {
        public async Task<JobResponse> Handle(JobRequest jobRequest, PerformContext? performContext = null)
        {
            JobResponse jobResponse = jobRequest.InitJobResponse(this, performContext);
            try
            {
                jobResponse.LogInformation("Job Started.");

                // process

                return jobResponse.HandleJobResponse(true, "Job Succeeded.");
            }
            catch (JobException jobException)
            {
                if (!string.IsNullOrEmpty(jobException.Message))
                    jobResponse.LogError(jobException.Message);
                throw;
            }
            catch (Exception exception)
            {
                jobResponse.LogException(exception, "Job Failed - Exception.");
                throw;
            }
        }
    }
}
