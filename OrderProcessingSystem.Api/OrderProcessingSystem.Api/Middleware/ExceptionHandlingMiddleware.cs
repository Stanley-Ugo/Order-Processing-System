using System.Net;
using System.Text.Json;
using OrderProcessingSystem.Application.Common.Exceptions;

namespace OrderProcessingSystem.Api.Middleware
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
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new
                    {
                        type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        title = "Validation Error",
                        status = 400,
                        errors = validationException.Errors
                    });
                    break;
                case ConflictException:
                    code = HttpStatusCode.Conflict;
                    result = JsonSerializer.Serialize(new
                    {
                        type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                        title = "Conflict",
                        status = 409,
                        detail = exception.Message
                    });
                    break;
                default:
                    result = JsonSerializer.Serialize(new
                    {
                        type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        title = "Server Error",
                        status = 500,
                        detail = "An error occurred while processing your request."
                    });
                    break;
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }


}
