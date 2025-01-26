using Hangfire.Server;
using JobManager.Application.Configurations;
using System.Text.Json.Serialization;

namespace JobManager.Application.Models.Jobs.Base
{
    public class JobResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string JobName { get; set; }
        [JsonIgnore] public PerformContext? PerformContext { get; set; }
        public JobRequest JobRequest { get; set; }


        // JobResponse yalnızca asagıdaki constructor ile initialize edilsin
        private JobResponse()
        {
        }

        public static JobResponse Init(JobRequest jobRequest, object handler, PerformContext? performContext)
        {
            return new JobResponse
            {
                JobName = !string.IsNullOrEmpty(handler?.GetType()?.Name) ? handler.GetType().Name.Replace(AppSettings.JobHandlerSuffix, "") : "",
                PerformContext = performContext,
                JobRequest = jobRequest
            };
        }
    }
}
