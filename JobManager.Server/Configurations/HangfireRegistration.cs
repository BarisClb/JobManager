using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using JobManager.Application.Configurations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace JobManager.Server.Configurations
{
    public static class HangfireRegistration
    {
        public static void RegisterHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return;

            var serviceProvider = services.BuildServiceProvider();
            var hangfireConnectionString = ReplaceInitialCatalog(configuration.GetConnectionString("MsSql") ?? throw new ArgumentNullException("Sql Connection String not found."), "Hangfire");
            services.AddHangfire(configuration => configuration
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireConnectionString, new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true,
                    QueuePollInterval = TimeSpan.FromSeconds(10),
                    SchemaName = $"JobManager{AppSettings.ProjectName}{((AppSettings.IsProdDb && !AppSettings.IsDebug) ? "Prod" : "")}"
                })
                .WithJobExpirationTimeout(TimeSpan.FromDays(7)));

            var hangfireServices = configuration.GetSection("Settings:JobServerSettings").Get<List<JobServerSetting>>();

            foreach (var serv in hangfireServices)
            {
                services.AddHangfireServer(options =>
                {
                    options.Queues = new[] { serv.ServerName };
                    options.ServerName = serv.ServerName;
                    options.WorkerCount = serv.WorkerCount;
                });
            }
        }

        public static void RegisterHangfireApps(this WebApplication app, IServiceCollection services, IConfiguration configuration)
        {
            if (!AppSettings.AllowHangfireRegistration())
                return;

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            GlobalConfiguration.Configuration.UseSerilogLogProvider().UseConsole();
        }

        private static string ReplaceInitialCatalog(string connectionString, string newCatalog)
        {
            return Regex.Replace(connectionString, @"(Initial Catalog\s*=\s*)([^;]+)", match => { return $"{match.Groups[1].Value}{newCatalog}"; });
        }
    }

    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
