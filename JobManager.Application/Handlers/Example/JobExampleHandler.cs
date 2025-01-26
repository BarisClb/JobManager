using Hangfire;
using Hangfire.Server;
using JobManager.Application.Configurations;
using JobManager.Application.Helpers.Extensions;
using JobManager.Application.Models.Exceptions;
using JobManager.Application.Models.Jobs.Base;
using JobManager.Application.Models.Jobs.Parameters;
using System.Text.Json;

namespace JobManager.Application.Handlers.Example
{
    public class JobExampleHandler
    {
        public async Task<JobResponse> Handle(JobRequest jobRequest, PerformContext? performContext = null)
        {
            JobResponse jobResponse = jobRequest.InitJobResponse(this, performContext);
            var jobParameters = jobRequest.InitParameters<JobExampleParameters>();
            if (!string.IsNullOrEmpty(jobRequest.Parameters) && jobParameters == null)
                return jobResponse.HandleJobResponse(false, "Failed to Deserialize JobParameters.");
            try
            {
                if (jobParameters == null)
                    return jobResponse.HandleJobResponse(false, "Invalid Job Parameters.");
                if (jobParameters.GroupList == null || jobParameters.GroupList.Count == 0)
                    return jobResponse.HandleJobResponse(false, "No Group provided.");

                foreach (var group in jobParameters.GroupList)
                {
                    var newJobRequest = jobRequest.CloneClass();
                    newJobRequest.ProcessId = Guid.NewGuid();
                    var newJobParameters = new JobExampleParameters()
                    {
                        TestParameter = jobParameters.TestParameter,
                        WaitTime = jobParameters.WaitTime,
                        GroupList = new() { group }
                    };

                    newJobRequest.Parameters = JsonSerializer.Serialize(newJobParameters);
                    if (AppSettings.IsDebug || performContext == null)
                        await JobExampleHandleByGroup(newJobRequest);
                    else
                        BackgroundJob.Enqueue<JobExampleHandler>(handler => handler.JobExampleHandleByGroup(jobRequest, null));
                }

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


        public async Task<JobResponse> JobExampleHandleByGroup(JobRequest jobRequest, PerformContext? performContext = null)
        {
            JobResponse jobResponse = jobRequest.InitJobResponse(this, performContext);
            var jobParameters = jobRequest.InitParameters<JobExampleParameters>();
            if (!string.IsNullOrEmpty(jobRequest.Parameters) && jobParameters == null)
                return jobResponse.HandleJobResponse(false, "Failed to Deserialize JobParameters.");
            if (jobParameters == null)
                jobParameters = new() { TestParameter = "", WaitTime = 1 };
            try
            {
                jobResponse.LogInformation("Job Started.");

                if (!string.IsNullOrEmpty(jobParameters.TestParameter))
                {
                    jobResponse.LogInformationCustom("{@Message} {@Test1} {@Test2} {@Test3} {@TestParameter}", new[] { "CustomParametersTest", "Test1Value", "Test2Value", "Test3Value", jobParameters.TestParameter });

                    if (jobParameters.TestParameter == "1")
                        throw new JobException("test");

                    if (jobParameters.TestParameter == "2")
                        throw new Exception("test");

                    if (jobParameters.TestParameter == "3")
                        return jobResponse.HandleJobResponse(false, "Job Failed");

                    if (jobParameters.TestParameter == "4")
                        return jobResponse.HandleJobResponse(true, "Job Succeeded");
                }

                if ((jobParameters?.WaitTime ?? 0) > 0)
                    await Task.Delay((jobParameters?.WaitTime ?? 0) * 1000);

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
