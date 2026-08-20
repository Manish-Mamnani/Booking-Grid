using BookingGrid.AuthService.Exceptions;
using System.Net;
using System.Text.Json;

namespace BookingGrid.AuthService.Middleware
{
    /// <summary>
    /// Converts authentication-domain exceptions into consistent JSON HTTP error responses.
    /// </summary>
    public class ExceptionHandlingMiddleware 
    {
        private readonly RequestDelegate _next;
        
        /// <summary>Initializes the middleware with the next request-pipeline delegate.</summary>
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>Executes the next pipeline component and maps known exceptions when they occur.</summary>
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

        /// <summary>Writes the HTTP status and JSON error body associated with an exception.</summary>
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
