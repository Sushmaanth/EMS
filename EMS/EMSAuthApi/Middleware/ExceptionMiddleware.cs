using System.Net;
using System.Text.Json;

namespace EMSAuthApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(context,HttpStatusCode.Unauthorized,ex.Message);
            }

            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(context,HttpStatusCode.NotFound,ex.Message);
            }

            catch (ArgumentException ex)
            {
                await HandleExceptionAsync(context,HttpStatusCode.BadRequest,ex.Message);
            }

            catch (Exception ex)
            {
                await HandleExceptionAsync(context,HttpStatusCode.InternalServerError,ex.Message);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context,HttpStatusCode statusCode,string message)
        {
            context.Response.ContentType ="application/json";

            context.Response.StatusCode =(int)statusCode;

            var response = new
            {
                Success = false,
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            var jsonResponse =JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
