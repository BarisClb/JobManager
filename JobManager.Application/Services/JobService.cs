using Hangfire;
using JobManager.Application.Configurations;
using JobManager.Application.Helpers.Services;
using JobManager.Application.Models.Jobs.Base;
using JobManager.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.Json;

namespace JobManager.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IConfiguration _configuration;
        private readonly JobSettings _jobSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly RecurringJobManager _recurringJobManager;

        public JobService(
            IConfiguration configuration,
            JobSettings jobSettings,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jobSettings = jobSettings ?? throw new ArgumentNullException(nameof(jobSettings));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            if (AppSettings.AllowHangfireRegistration())
                _recurringJobManager = new RecurringJobManager();
        }


        public string UpdateAllJobsWithDefaultTimes(string? prodRequestCode)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return "Hangfire is not Registered.";

            if (AppSettings.IsProdDb)
            {
                var prodRequestCodeCheck = JobServiceHelper.CheckProdRequestCode(prodRequestCode);
                if (!string.IsNullOrEmpty(prodRequestCodeCheck))
                    return $"""This process will effect the Prod DB. If you want to proceed, use "{prodRequestCodeCheck}" as ProdRequestCode and try again.""";
            }

            return UpdateAllJobsToDefaultTimes();
        }

        public string DeactivateAllJobs(string? prodRequestCode)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return "Hangfire is not Registered.";

            if (_jobSettings?.ProjectJobs == null || !_jobSettings.ProjectJobs.Any())
                return "No Project Jobs found.";

            if (AppSettings.IsProdDb)
            {
                var prodRequestCodeCheck = JobServiceHelper.CheckProdRequestCode(prodRequestCode);
                if (!string.IsNullOrEmpty(prodRequestCodeCheck))
                    return $"""This process will effect the Prod DB. If you want to proceed, use "{prodRequestCodeCheck}" as ProdRequestCode and try again.""";
            }

            var allJobs = JobServiceHelper.GetAllJobs();

            foreach (var projectJob in _jobSettings.ProjectJobs)
            {
                if (string.IsNullOrEmpty(projectJob))
                    continue;

                var job = allJobs.FirstOrDefault(j => j.Name == $"{projectJob}{AppSettings.JobRecurringSuffix}");
                if (job == null)
                    continue;

                var jobRegistrationSetting = _jobSettings?.JobRegistrationSettings?.RegistrationSettings?.FirstOrDefault(r => r.JobName == projectJob);
                if (jobRegistrationSetting == null || !jobRegistrationSetting.IsWorksByDefault)
                {
                    _recurringJobManager.RemoveIfExists(projectJob);
                    continue;
                }

                Hangfire.Common.Job jobTask = new Hangfire.Common.Job(job, job.GetMethod(AppSettings.JobRecurringMethodName), new object?[] { null });
                _recurringJobManager.AddOrUpdate(projectJob, jobTask, "0 5 31 2 *", TimeZoneInfo.Utc);
            }

            return "All jobs have been deactivated.";
        }

        public string RemoveAllJobs(string? prodRequestCode)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return "Hangfire is not Registered.";

            if (_jobSettings?.ProjectJobs == null || !_jobSettings.ProjectJobs.Any())
                return "No Project Jobs found.";

            if (AppSettings.IsProdDb)
            {
                var prodRequestCodeCheck = JobServiceHelper.CheckProdRequestCode(prodRequestCode);
                if (!string.IsNullOrEmpty(prodRequestCodeCheck))
                    return $"""This process will effect the Prod DB. If you want to proceed, use "{prodRequestCodeCheck}" as ProdRequestCode and try again.""";
            }

            var allJobs = JobServiceHelper.GetAllJobs();

            foreach (var projectJob in _jobSettings.ProjectJobs)
            {
                if (string.IsNullOrEmpty(projectJob))
                    continue;

                var job = allJobs.FirstOrDefault(j => j.Name == $"{projectJob}{AppSettings.JobRecurringSuffix}");
                if (job == null)
                    continue;

                _recurringJobManager.RemoveIfExists(projectJob);
            }

            return "All jobs have been removed.";
        }

        public string UpdateJobTimeByName(string jobName, string cronExpression, string? prodRequestCode)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return "Hangfire is not Registered.";

            if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(cronExpression))
                return "Invalid input.";

            if (!_jobSettings.ProjectJobs.Contains(jobName))
                return $"'{jobName}' was not found in Project Jobs.";

            if (AppSettings.IsProdDb)
            {
                var prodRequestCodeCheck = JobServiceHelper.CheckProdRequestCode(prodRequestCode);
                if (!string.IsNullOrEmpty(prodRequestCodeCheck))
                    return $"""This process will effect the Prod DB. If you want to proceed, use "{prodRequestCodeCheck}" as ProdRequestCode and try again.""";
            }

            var jobs = JobServiceHelper.GetAllJobs();

            var jobType = jobs.FirstOrDefault(type => type.Name == $"{jobName}{AppSettings.JobRecurringSuffix}");
            if (jobType == null)
                return $"'{jobName}' it not a Recurring Job.";

            var jobMethod = jobType.GetMethod(AppSettings.JobRecurringMethodName);
            if (jobMethod == null)
                return $"'{AppSettings.JobRecurringMethodName}' method not found for '{jobName}'.";

            var job = new Hangfire.Common.Job(jobType, jobMethod, new object?[] { null });

            if (cronExpression == "-1")
            {
                _recurringJobManager.RemoveIfExists(jobName);
                return $"'{jobName}' has been removed.";
            }
            else if (cronExpression == "0")
            {
                _recurringJobManager.AddOrUpdate(jobName, job, "0 5 31 2 *", TimeZoneInfo.Utc);
                return $"'{jobName}' has been updated with deactivation Cron time: '{AppSettings.JobDeactivationCronTime}'.";
            }
            else if (cronExpression == "1")
            {
                var defaultCron = _configuration[$"Settings:JobCronSettings:{jobName}"];
                if (string.IsNullOrEmpty(defaultCron))
                    return $"Default CronTime not found for '{jobName}' in 'appsettings.Settings.JobCronSettings'.";
                _recurringJobManager.AddOrUpdate(jobName, job, defaultCron, TimeZoneInfo.Utc);
                return $"'{jobName}' has been updated with default Cron expression: '{defaultCron}'.";
            }

            _recurringJobManager.AddOrUpdate(jobName, job, cronExpression, TimeZoneInfo.Utc);
            return $"'{jobName}' has been updated with input Cron expression: '{cronExpression}'.";
        }

        public async Task<object> StartJobByJobName(string jobName, JobRequestEndpoint? jobRequestEndpoint = default)
        {
            if (string.IsNullOrEmpty(jobName))
                return "Job name is missing.";

            if (jobRequestEndpoint == null)
                return "Job request body is missing.";

            if (!AppSettings.IsDebug || AppSettings.AllowHangfireRegistration())
            {
                if (_jobSettings?.ProjectJobs == null || !_jobSettings.ProjectJobs.Any())
                    return "No Project Jobs found.";

                if (!_jobSettings.ProjectJobs.Contains(jobName))
                    return $"'{jobName}' was not found in Project Jobs.";
            }

            if (AppSettings.IsProdDb)
            {
                var prodRequestCodeCheck = JobServiceHelper.CheckProdRequestCode(jobRequestEndpoint.ProdRequestCode);
                if (!string.IsNullOrEmpty(prodRequestCodeCheck))
                    return $"""This process will effect the Prod DB. If you want to proceed, use "{prodRequestCodeCheck}" as ProdRequestCode and try again.""";
            }

            if (AppSettings.AllowHangfireRegistration())
            {
                var recurringTypeName = $"{jobName}{AppSettings.JobRecurringSuffix}";
                var recurringType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == recurringTypeName);

                if (recurringType == null)
                    return $"Handler '{recurringTypeName}' not found in loaded assemblies.";

                var methodInfo = recurringType.GetMethod(AppSettings.JobRecurringMethodName);
                if (methodInfo == null)
                    return $"Method '{AppSettings.JobRecurringMethodName}' not found on Recurring: '{recurringTypeName}'.";

                var recurringInstance = _serviceProvider.GetService(recurringType) ?? Activator.CreateInstance(recurringType);
                if (recurringInstance == null)
                    return $"Handler instance for '{recurringTypeName}' could not be created.";

                methodInfo.Invoke(recurringInstance, new object?[] { jobRequestEndpoint.Parameters });

                return "Job enqueued successfully.";
            }
            else
            {
                var handlerTypeName = $"{jobName}{AppSettings.JobHandlerSuffix}";
                var handlerType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == handlerTypeName);

                if (handlerType == null)
                    return $"Handler '{handlerTypeName}' not found in loaded assemblies.";

                var methodInfo = handlerType.GetMethod(AppSettings.JobMethodName);
                if (methodInfo == null)
                    return $"Method '{AppSettings.JobMethodName}' not found on handler '{handlerTypeName}'.";

                var handlerInstance = _serviceProvider.GetService(handlerType) ?? Activator.CreateInstance(handlerType);
                if (handlerInstance == null)
                    return $"Handler instance for '{handlerTypeName}' could not be created.";

                JobRequest jobRequest = new()
                {
                    ProcessId = Guid.NewGuid(),
                    IsProd = jobRequestEndpoint.IsProd,
                    Parameters = jobRequestEndpoint.Parameters == null ? null : JsonSerializer.Serialize(jobRequestEndpoint.Parameters)
                };

                var parameters = methodInfo.GetParameters();
                var args = parameters.Select(p =>
                {
                    if (p.ParameterType == typeof(JobRequest))
                        return jobRequest;
                    return GetDefaultValue(p.ParameterType);
                }).ToArray();

                var result = methodInfo.Invoke(handlerInstance, args);

                if (result is Task taskResult)
                {
                    await taskResult;
                    if (taskResult.GetType().IsGenericType)
                    {
                        return taskResult?.GetType()?.GetProperty("Result")?.GetValue(taskResult) ?? new();
                    }

                    return "Job completed successfully.";
                }
            }

            return "Method ended without a response.";
        }

        public string InitJobs()
        {
            if (!AppSettings.AllowHangfireRegistration())
                return $"Hangfire is not Registered.";

            return UpdateAllJobsToDefaultTimes();
        }


        private string UpdateAllJobsToDefaultTimes()
        {
            if (_jobSettings?.ProjectJobs == null || !_jobSettings.ProjectJobs.Any())
                return "No Project Jobs found.";

            var allJobs = JobServiceHelper.GetAllJobs();

            foreach (var projectJob in _jobSettings.ProjectJobs)
            {
                if (string.IsNullOrEmpty(projectJob))
                    continue;

                var job = allJobs.FirstOrDefault(j => j.Name == $"{projectJob}{AppSettings.JobRecurringSuffix}");
                if (job == null)
                    continue;

                var jobRegistrationSetting = _jobSettings?.JobRegistrationSettings?.RegistrationSettings?.FirstOrDefault(r => r.JobName == projectJob);
                if (jobRegistrationSetting == null || !jobRegistrationSetting.IsWorksByDefault)
                {
                    _recurringJobManager.RemoveIfExists(projectJob);
                    continue;
                }

                var jobCronSetting = _jobSettings?.JobCronSettings?.CronSettings?.FirstOrDefault(cs => cs.JobName == projectJob);
                if (string.IsNullOrEmpty(jobCronSetting?.DefaultCronTime))
                    continue;

                Hangfire.Common.Job jobTask = new Hangfire.Common.Job(job, job.GetMethod(AppSettings.JobRecurringMethodName), new object?[] { null });
                _recurringJobManager.AddOrUpdate(projectJob, jobTask, jobCronSetting.DefaultCronTime, TimeZoneInfo.Utc);
            }

            return "All jobs have been updated with default times.";
        }

        public object? GetJobParametersByJobName(string jobName)
        {
            var type = Type.GetType($"JobManager.Application.Models.Jobs.Parameters.{jobName}{AppSettings.JobParametersSuffix}");
            if (type == null)
                return null;

            return Activator.CreateInstance(type);
        }

        public List<Dictionary<string, object>> GetAllAppConstants()
        {
            var result = new List<Dictionary<string, object>>();
            var assembly = Assembly.GetExecutingAssembly();
            var namespacePrefix = "JobManager.Application.Helpers.Constants";
            var suffix = "Constants";

            var types = assembly.GetTypes()
                .Where(t => t.IsClass
                            && t.Namespace == namespacePrefix
                            && t.Name.EndsWith(suffix))
                .ToList();

            foreach (var type in types)
            {
                try
                {
                    var instance = _serviceProvider.GetService(type);
                    if (instance != null)
                    {
                        result.Add(new Dictionary<string, object>
                    {
                        { type.Name, instance }
                    });
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return result;
        }

        private object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }
    }
}
