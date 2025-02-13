using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TrojWebApp
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);  // Kör nästa middleware/request
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation($"Request till {context.Request.Path} tog {elapsedMs} ms");
        }
    }
}
