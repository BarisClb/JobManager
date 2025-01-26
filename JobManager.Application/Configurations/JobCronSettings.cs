namespace JobManager.Application.Configurations
{
    public class JobCronSettings
    {
        public List<JobCronSetting> CronSettings { get; set; }
    }

    public class JobCronSetting
    {
        public string JobName { get; set; }
        public string DefaultCronTime { get; set; }
    }
}
