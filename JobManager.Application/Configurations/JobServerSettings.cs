namespace JobManager.Application.Configurations
{
    public class JobServerSettings
    {
        public List<JobServerSetting> ServerSettings { get; set; }
    }

    public class JobServerSetting
    {
        public string ServerName { get; set; }
        public int WorkerCount { get; set; }
    }
}
