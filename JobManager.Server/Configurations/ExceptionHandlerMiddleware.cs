using JobManager.Application.Configurations;
using JobManager.Application.Models.Exceptions;
using Serilog;
using System.Text;
using System.Text.Json;

namespace JobManager.Server.Configurations
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                string requestBody = null;
                if (context.Request.Method != "GET")
                {
                    requestBody = await GetRawBodyAsync(context.Request);
                }

                var response = context.Response;
                var exceptionResponse = new ExceptionResponse() { Exception = exception };
                var responseBody = JsonSerializer.Serialize(exceptionResponse);

                if (!AppSettings.IsDebug)
                    Log.Error("{@UtcTime} {@Method} {@Request} {@Response}", DateTime.UtcNow, "ExceptionHandlerMiddleware", requestBody, responseBody);

                await response.WriteAsync(responseBody);
            }
        }


        private async Task<string> GetRawBodyAsync(HttpRequest request, Encoding encoding = null)
        {
            using (var buffer = new MemoryStream())
            {
                request.Body = buffer;
                buffer.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(buffer);
                using (var bufferReader = new StreamReader(buffer))
                {
                    return await bufferReader.ReadToEndAsync();
                }
            }
        }
    }
}
