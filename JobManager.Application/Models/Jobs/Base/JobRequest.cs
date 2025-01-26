using Hangfire.Server;
using System.ComponentModel;
using System.Text.Json;

namespace JobManager.Application.Models.Jobs.Base
{
    public class JobRequest : ICloneable
    {
        public Guid ProcessId { get; set; } = Guid.NewGuid();
        [DefaultValue(false)] public bool IsProd { get; set; }
        [DefaultValue(null)] public string? Parameters { get; set; }


        public static JobRequest Recurring(object? parameters = null)
        {
            return new JobRequest()
            {
                IsProd = true,
                Parameters = parameters != null ? JsonSerializer.Serialize(parameters) : null
            };
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public JobRequest CloneClass()
        {
            return (JobRequest)this.Clone();
        }
    }

    public static class JobRequestHelper
    {
        public static JobResponse InitJobResponse(this JobRequest jobRequest, object handler, PerformContext? performContext)
            => JobResponse.Init(jobRequest, handler, performContext);

        public static T? InitParameters<T>(this JobRequest jobRequest)
        {
            if (string.IsNullOrEmpty(jobRequest.Parameters))
                return default;

            try
            {
                var jsonDocument = JsonDocument.Parse(jobRequest.Parameters);
                var providedProperties = jsonDocument.RootElement.EnumerateObject().Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var expectedProperties = typeof(T).GetProperties().Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (!providedProperties.IsSubsetOf(expectedProperties))
                    return default;
                return JsonSerializer.Deserialize<T>(jobRequest.Parameters, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }

    public class JobRequestEndpoint
    {
        [DefaultValue(false)] public bool IsProd { get; set; }
        [DefaultValue(null)] public object? Parameters { get; set; }
        [DefaultValue("")] public string? ProdRequestCode { get; set; }
    }
}
