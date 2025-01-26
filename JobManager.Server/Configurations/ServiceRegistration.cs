using JobManager.Application.Configurations;
using System.Reflection;

namespace JobManager.Server.Configurations
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, IConfigurationBuilder configuration)
        {
            AppSettings.Init();

            configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Settings/Environment/appsettings.{AppSettings.AppEnvironment?.ToLower()}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Settings/Project/appsettings.{AppSettings.ProjectName?.ToLower()}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Settings/Project/appsettings.{AppSettings.ProjectName?.ToLower()}-{AppSettings.AppEnvironment?.ToLower()}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public static void RegisterApps(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
