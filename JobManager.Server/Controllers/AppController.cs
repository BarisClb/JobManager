using JobManager.Application.Configurations;
using JobManager.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobManager.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppController : ApiControllerBase
    {
        private readonly JobSettings _jobSettings;
        private readonly IJobService _jobService;

        public AppController(
            JobSettings jobSettings,
            IJobService jobService)
        {
            _jobSettings = jobSettings;
            _jobService = jobService;
        }


        /// <summary>
        /// Job Settings for The Project. (Active Jobs, Default Times, Server Settings and Job List for Debugging)
        /// </summary>
        [HttpGet("JobSettings")]
        public IActionResult GetJobSettings()
        {
            return Ok(_jobSettings);
        }

        /// <summary>
        /// App Settings for The Project. (Service Environments, Project Standards, Run Mode)
        /// </summary>
        [HttpGet("AppSettings")]
        public IActionResult GetAppSettings()
        {
            return Ok(AppSettings.GetAppSettings());
        }

        /// <summary>
        /// App Constants that are Shared by Jobs
        /// </summary>
        [HttpGet("AppConstants")]
        public IActionResult GetAppConstants()
        {
            return Ok(_jobService.GetAllAppConstants());
        }
    }
}
