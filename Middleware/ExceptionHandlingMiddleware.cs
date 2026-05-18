using System.Net;
using System.Text.Json;

namespace TaskManagement.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            var errorCode = ex switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var errorResponse = new ErrorResponse
            {
                StatusCode = errorCode,
                Message = ex.Message,
                TraceId = context.TraceIdentifier
            };

            _logger.LogError(ex, "Unhandled exception occurred. StatusCode: {StatusCode}, TraceId: {TraceId}", errorCode, errorResponse.TraceId);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorCode;

            var response = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(response);
        }
    }
}
