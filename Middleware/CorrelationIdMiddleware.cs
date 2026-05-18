namespace TaskManagement.Api.Middleware
{
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-ID";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetCorrelationId(context);
            context.TraceIdentifier = correlationId;
            context.Response.Headers[HeaderName] = correlationId;

            using (_logger.BeginScope("CorrelationId: {CorrelationId}", correlationId))
            {
                _logger.LogInformation(
                    "Request started. Method: {Method}, Path: {Path}, CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    correlationId);

                await _next(context);

                _logger.LogInformation(
                    "Request completed. StatusCode: {StatusCode}, CorrelationId: {CorrelationId}",
                    context.Response.StatusCode,
                    correlationId);
            }
        }

        private static string GetCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(HeaderName, out var correlationId) &&
                !string.IsNullOrWhiteSpace(correlationId))
            {
                return correlationId.ToString();
            }

            return Guid.NewGuid().ToString();
        }
    }
}
