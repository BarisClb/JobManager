using JobManager.Application.Models.Jobs.Base;
using JobManager.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobManager.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class JobController : ApiControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }


        /// <summary>
        /// If Hangfire is not Registered, it will Invoke the Method for Debugging. Otherwise, it will Enqueue the job.
        /// </summary>
        [HttpPost("StartJobByJobName")]
        public async Task<IActionResult> StartJobByJobName(string jobName, [FromBody] JobRequestEndpoint? jobRequestEndpoint = default)
        {
            return Ok(await _jobService.StartJobByJobName(jobName, jobRequestEndpoint));
        }

        /// <summary>
        /// Get Job parameters.
        /// </summary>
        [HttpGet("GetJobParametersJobByName")]
        public IActionResult GetJobParametersJobByName(string jobName)
        {
            return Ok(_jobService.GetJobParametersByJobName(jobName));
        }

        /// <summary>
        /// Update a Single Job to specified Cron Expression.
        /// </summary>
        /// <param name="cronExpression"> You can send '-1' to Remove Job, 0 to Deactivate Job, 1: Set to Default Time (appsettings) or CronExpression, to set it to a specific Schedule.</param>
        [HttpPatch("UpdateJobTimeByName")]
        public IActionResult UpdateJobTimeByName(string jobName, string cronExpression, string? prodRequestCode = default)
        {
            return Ok(_jobService.UpdateJobTimeByName(jobName, cronExpression, prodRequestCode));
        }

        /// <summary>
        /// Update All Jobs to Cron Expressions specified in appsettings
        /// </summary>
        [HttpPatch("UpdateAllJobsWithDefaultTimes")]
        public IActionResult UpdateAllJobsWithDefaultTimes(string? prodRequestCode = default)
        {
            return Ok(_jobService.UpdateAllJobsWithDefaultTimes(prodRequestCode));
        }

        /// <summary>
        /// Manual Deactivation of All Jobs. (Update All Jobs time to 31st day of Februrary)
        /// </summary>
        [HttpDelete("DeactivateAllJobs")]
        public IActionResult DeactivateAllJobs(string? prodRequestCode = default)
        {
            return Ok(_jobService.DeactivateAllJobs(prodRequestCode));
        }

        /// <summary>
        /// Remove All Jobs from Recurring Jobs.
        /// </summary>
        [HttpDelete("RemoveAllJobs")]
        public IActionResult RemoveAllJobs(string? prodRequestCode = default)
        {
            return Ok(_jobService.RemoveAllJobs(prodRequestCode));
        }
    }
}
