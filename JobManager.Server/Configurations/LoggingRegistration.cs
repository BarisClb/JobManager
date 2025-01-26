using JobManager.Application.Configurations;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System.Text;

namespace JobManager.Server.Configurations
{
    public static class LoggingRegistration
    {
        public static void RegisterLogging(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterElasticsearch(configuration);
        }

        public static void RegisterElasticsearch(IConfiguration configuration)
        {
            StringBuilder indexFormat = new("jobmanager");
            indexFormat.Append($"-{AppSettings.AppEnvironment?.ToLower().Replace(".", "-")}");
            indexFormat.Append($"{(string.IsNullOrEmpty(AppSettings.ProjectName) ? "" : $"-{AppSettings.ProjectName.ToLower().Replace(".", "-")}")}");
            indexFormat.Append($"-{DateTime.UtcNow:yyyy-MM-dd}");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration.GetConnectionString("Elasticsearch") ?? throw new ArgumentNullException("Elastic url not found.")))
                {
                    CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                    ModifyConnectionSettings = c => c.ServerCertificateValidationCallback((o, certificate, arg3, arg4) => true),
                    AutoRegisterTemplate = true,
                    IndexFormat = indexFormat.ToString(),
                })
                .Enrich.WithProperty("Environment", AppSettings.AppEnvironment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
