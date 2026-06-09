using System.Net;
using System.Text.Json;

namespace EMSAuthApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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

            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorised access attempt. Path: {Path}", context.Request.Path);

                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }

            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found. Path: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }

            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad argument. Path: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. Path: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                Success = false,
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
