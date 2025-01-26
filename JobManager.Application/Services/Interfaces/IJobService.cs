using JobManager.Application.Models.Jobs.Base;

namespace JobManager.Application.Services.Interfaces
{
    public interface IJobService
    {
        string UpdateAllJobsWithDefaultTimes(string? prodRequestCode);
        string DeactivateAllJobs(string? prodRequestCode);
        string RemoveAllJobs(string? prodRequestCode);
        string UpdateJobTimeByName(string jobName, string cronExpression, string? prodRequestCode);
        Task<object> StartJobByJobName(string jobName, JobRequestEndpoint? jobRequestEndpoint = default);
        string InitJobs();
        object? GetJobParametersByJobName(string jobName);
        List<Dictionary<string, object>> GetAllAppConstants();
    }
}
