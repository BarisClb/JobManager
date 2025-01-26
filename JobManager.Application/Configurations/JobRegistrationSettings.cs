namespace JobManager.Application.Configurations
{
    public class JobRegistrationSettings
    {
        public List<JobRegistrationSetting> RegistrationSettings { get; set; }
    }

    public class JobRegistrationSetting
    {
        public string JobName { get; set; }
        public bool IsWorksByDefault { get; set; }
    }
}
