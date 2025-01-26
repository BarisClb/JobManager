using Hangfire.Console;
using JobManager.Application.Models.Enums;
using JobManager.Application.Models.Exceptions;
using JobManager.Application.Models.Jobs.Base;
using System.Text.Json;

namespace JobManager.Application.Helpers.Extensions
{
    public static class JobResponseExtensions
    {
        private static string _standartTemplate = "{@UtcTime} {@ProcessId} {@JobId} {@JobName} {@JobRequest}";

        public static JobResponse HandleJobResponse(this JobResponse jobResponse, bool isSuccess, string message)
        {
            jobResponse.IsSuccess = isSuccess;
            jobResponse.Message = message;

            if (isSuccess)
                jobResponse.LogInformation(JsonSerializer.Serialize(jobResponse));
            else
                jobResponse.LogError(JsonSerializer.Serialize(jobResponse));

            if (jobResponse.PerformContext != null)
                jobResponse.PerformContext.WriteLine(message);

            if (!jobResponse.IsSuccess && jobResponse.JobRequest.IsProd)
                throw new JobException();

            return jobResponse;
        }

        public static void LogInformation(this JobResponse jobResponse, string logMessage) => jobResponse.Log(LogType.Information, logMessage);
        public static void LogWarning(this JobResponse jobResponse, string logMessage) => jobResponse.Log(LogType.Warning, logMessage);
        public static void LogFatal(this JobResponse jobResponse, string logMessage) => jobResponse.Log(LogType.Fatal, logMessage);
        public static void LogError(this JobResponse jobResponse, string logMessage) => jobResponse.Log(LogType.Error, logMessage);

        public static void LogInformationCustom(this JobResponse jobResponse, string template, string[] variables) => jobResponse.LogCustom(LogType.Information, template, variables);
        public static void LogWarningCustom(this JobResponse jobResponse, string template, string[] variables) => jobResponse.LogCustom(LogType.Warning, template, variables);
        public static void LogFatalCustom(this JobResponse jobResponse, string template, string[] variables) => jobResponse.LogCustom(LogType.Fatal, template, variables);
        public static void LogErrorCustom(this JobResponse jobResponse, string template, string[] variables) => jobResponse.LogCustom(LogType.Error, template, variables);

        public static void LogException(this JobResponse jobResponse, Exception exception, string logMessage)
        {
            if (jobResponse.PerformContext != null)
                jobResponse.PerformContext.WriteLine(logMessage);

            if (jobResponse.JobRequest.IsProd)
            {
                var parameters = CreateLogParameters(jobResponse, new[] { logMessage });
                Serilog.Log.Error(exception, _standartTemplate, parameters);
            }
        }

        public static void LogSqlQuery(this JobResponse jobResponse, string sqlQuery, object? sqlParameters = null)
        {
            if (jobResponse.JobRequest.IsProd)
            {
                var parameters = CreateLogParameters(jobResponse, new[] { sqlQuery, (sqlParameters != null ? JsonSerializer.Serialize(sqlParameters) : "") });
                Serilog.Log.Information(_standartTemplate + " {@SqlQuery} {@Parameters}", parameters);
            }
        }

        private static void Log(this JobResponse jobResponse, LogType logType, string logMessage)
        {
            if (jobResponse.PerformContext != null)
                jobResponse.PerformContext.WriteLine(logMessage);

            if (jobResponse.JobRequest.IsProd)
            {
                string logTemplate = _standartTemplate + " {@Message}";
                var parameters = CreateLogParameters(jobResponse, new[] { logMessage });

                switch (logType)
                {
                    case LogType.Information:
                        Serilog.Log.Information(logTemplate, parameters);
                        break;
                    case LogType.Warning:
                        Serilog.Log.Warning(logTemplate, parameters);
                        break;
                    case LogType.Fatal:
                        Serilog.Log.Fatal(logTemplate, parameters);
                        break;
                    case LogType.Error:
                        Serilog.Log.Error(logTemplate, parameters);
                        break;
                }
            }
        }

        private static void LogCustom(this JobResponse jobResponse, LogType logType, string template, string[] variables)
        {
            if (jobResponse.PerformContext != null)
                jobResponse.PerformContext.WriteLine(JsonSerializer.Serialize(new { Template = template, Variables = variables }));

            if (jobResponse.JobRequest.IsProd)
            {
                string logTemplate = _standartTemplate + $" {template}";
                var parameters = CreateLogParameters(jobResponse, variables);

                switch (logType)
                {
                    case LogType.Information:
                        Serilog.Log.Information(logTemplate, parameters);
                        break;
                    case LogType.Warning:
                        Serilog.Log.Warning(logTemplate, parameters);
                        break;
                    case LogType.Fatal:
                        Serilog.Log.Fatal(logTemplate, parameters);
                        break;
                    case LogType.Error:
                        Serilog.Log.Error(logTemplate, parameters);
                        break;
                }
            }
        }

        private static object?[] CreateLogParameters(JobResponse jobResponse, string[] variables)
        {
            var asd = new List<object?>()
        {
            DateTime.UtcNow,
            jobResponse.JobRequest.ProcessId,
            jobResponse.PerformContext?.BackgroundJob?.Id,
            jobResponse.JobName,
            JsonSerializer.Serialize(jobResponse.JobRequest)
        };
            asd.AddRange(variables);
            return asd.ToArray();
        }
    }
}
