using BookingGrid.AuthService.Exceptions;
using System.Net;
using System.Text.Json;

namespace BookingGrid.AuthService.Middleware
{
    public class ExceptionHandlingMiddleware 
    {
        private readonly RequestDelegate _next;
        
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception exception)
            {
                    await HandleExceptionAsync(exception,context);
            }
        }

        public static Task HandleExceptionAsync(Exception exception,HttpContext context)
        {
            var statusCode = exception switch
            {
                UserAlreadyExistsException => HttpStatusCode.Conflict,
                InvalidCredentialsException => HttpStatusCode.Unauthorized,
                UserNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = JsonSerializer.Serialize(new { message = exception.Message });
            return context.Response.WriteAsync(response);
        }
    }
}
