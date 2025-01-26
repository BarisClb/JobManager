using JobManager.Application.Configurations;
using JobManager.Application.Helpers.Services;
using JobManager.Application.Services;
using JobManager.Application.Services.Interfaces;
using JobManager.Application.ServicesDb;
using JobManager.Application.ServicesDb.Interfaces;
using System.Reflection;

namespace JobManager.Server.Configurations
{
    public static class ApplicationServiceRegistration
    {
        public static void RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterConfigurations(services, configuration);
            RegisterConstants(services, configuration);
            RegisterServices(services);
            RegisterHandlers(services);
            RegisterRecurrings(services, configuration);
        }

        // Inject Services - Manually handled because of cross-dependency
        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ISqlService, MsSqlService>();
            services.AddScoped<ICacheService, RedisService>();
            services.AddScoped<IJobService, JobService>();
        }

        // Inject Handlers to Debug Jobs via Swagger
        private static void RegisterHandlers(IServiceCollection services)
        {
            var assembly = typeof(IJobHandler).Assembly;

            var handlers = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IJobHandler).IsAssignableFrom(t))
                .ToList();

            foreach (var handler in handlers)
                services.AddScoped(handler);
        }

        // Inject Recurrings to Enqueue Jobs via Swagger
        private static void RegisterRecurrings(IServiceCollection services, IConfiguration configuration)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return;

            var jobSettings = GetJobRegistrationSettings(configuration);
            var allRecurrings = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IJobRecurring).IsAssignableFrom(t))
                .ToList();

            foreach (var recurring in allRecurrings)
            {
                var jobName = recurring.Name.Replace(AppSettings.JobRecurringSuffix, "");
                var isActive = jobSettings.RegistrationSettings.Any(js => js.JobName == jobName && js.IsWorksByDefault);

                if (isActive)
                    services.AddScoped(recurring);
            }
        }

        // Inject App Constants
        private static void RegisterConstants(IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(IJobHandler).Assembly;
            var constants = assembly.GetTypes()
                .Where(t => t.IsClass && t.Namespace == "JobManager.Application.Helpers.Constants" && t.Name.EndsWith("Constants"))
                .ToList();

            foreach (var constant in constants)
            {
                var configSection = configuration.GetSection($"Constants:{constant.Name}");
                var instance = configSection.Get(constant) ?? Activator.CreateInstance(constant);
                services.AddSingleton(constant, instance!);
            }
        }

        private static void RegisterConfigurations(IServiceCollection services, IConfiguration configuration)
        {
            var allJobs = JobServiceHelper.GetAllJobs();
            var jobRegistrationSettings = GetJobRegistrationSettings(configuration);
            services.AddSingleton(new JobSettings()
            {
                ProjectJobs = jobRegistrationSettings.RegistrationSettings.Select(rs => rs.JobName).ToList(),
                JobRegistrationSettings = jobRegistrationSettings,
                JobCronSettings = GetJobCronSettings(configuration, jobRegistrationSettings),
                JobServerSettings = GetJobServerSettings(configuration, jobRegistrationSettings),
                AllJobs = !AppSettings.AllowHangfireRegistration() ? allJobs.Select(rs => rs.Name.Replace(AppSettings.JobRecurringSuffix, "")).ToList() : new()
            });
        }

        private static JobRegistrationSettings GetJobRegistrationSettings(IConfiguration configuration)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return new() { RegistrationSettings = new List<JobRegistrationSetting>() };

            var jobsRegistrationSettingsSection = configuration.GetSection("Settings:JobRegistrationSettings");
            var registrationSettingsList = jobsRegistrationSettingsSection.Get<Dictionary<string, bool>>()
                ?.Where(jrs => jrs.Value)
                ?.OrderBy(jrs => jrs.Key)
                ?.Select(kvp => new JobRegistrationSetting
                {
                    JobName = kvp.Key,
                    IsWorksByDefault = kvp.Value
                })
                ?.ToList() ?? new List<JobRegistrationSetting>();
            return new JobRegistrationSettings() { RegistrationSettings = registrationSettingsList };
        }

        private static JobCronSettings GetJobCronSettings(IConfiguration configuration, JobRegistrationSettings jobRegistrationSettings)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return new() { CronSettings = new List<JobCronSetting>() };

            var jobCronSettingsSection = configuration.GetSection("Settings:JobCronSettings");
            var cronSettingsList = jobCronSettingsSection.Get<Dictionary<string, string>>()
                ?.Where(cs => jobRegistrationSettings.RegistrationSettings.Any(rs => rs.JobName == cs.Key))
                ?.OrderBy(cs => cs.Key)
                ?.Select(kvp => new JobCronSetting
                {
                    JobName = kvp.Key,
                    DefaultCronTime = kvp.Value
                })
                ?.ToList() ?? new List<JobCronSetting>();
            return new JobCronSettings() { CronSettings = cronSettingsList };
        }

        private static JobServerSettings GetJobServerSettings(IConfiguration configuration, JobRegistrationSettings jobRegistrationSettings)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return new JobServerSettings { ServerSettings = new List<JobServerSetting>() };

            var jobServerSettingsSection = configuration.GetSection("Settings:JobServerSettings");
            var jobServerSettingsList = jobServerSettingsSection.Get<List<JobServerSetting>>() ?? new List<JobServerSetting>();
            var filteredSettings = jobServerSettingsList
                .OrderBy(ss => ss.ServerName)
                .ToList();

            return new JobServerSettings { ServerSettings = filteredSettings };
        }
    }
}
