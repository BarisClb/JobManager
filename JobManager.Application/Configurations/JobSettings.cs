namespace JobManager.Application.Configurations
{
    public class JobSettings
    {
        public List<string> ProjectJobs { get; set; }
        public JobCronSettings JobCronSettings { get; set; }
        public JobRegistrationSettings JobRegistrationSettings { get; set; }
        public JobServerSettings JobServerSettings { get; set; }
        public List<string> AllJobs { get; set; }
    }
}
