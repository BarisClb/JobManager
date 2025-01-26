using JobManager.Application.Configurations;
using JobManager.Server.Configurations;

namespace JobManager.Server.Recurrings.Shared
{
    public class UpdateElasticLoggingRecurring : IJobRecurring
    {
        private readonly IConfiguration _configuration;

        public UpdateElasticLoggingRecurring(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Start(object? parameters = null)
        {
            LoggingRegistration.RegisterElasticsearch(_configuration);
        }
    }
}
