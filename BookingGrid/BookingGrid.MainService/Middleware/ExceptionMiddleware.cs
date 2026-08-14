using BookingGrid.MainService.Exceptions;
using System.Net;
using System.Text.Json;

namespace BookingGrid.MainService.Middleware
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
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private static Task HandleException(HttpContext context, Exception ex)
        {
            var statusCode = ex switch
            {
                HotelNotFoundException => HttpStatusCode.NotFound,
                RoomNotFoundException => HttpStatusCode.NotFound,
                InvalidHotelOperationException => HttpStatusCode.BadRequest,
                InvalidRatingException => HttpStatusCode.BadRequest,
                RoomNotAvailableException => HttpStatusCode.BadRequest,
                InvalidBookingDatesException => HttpStatusCode.BadRequest,
                BookingNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedBookingAccessException => HttpStatusCode.Forbidden,
                BookingConflictException => HttpStatusCode.Conflict,
                BookingAlreadyCancelledException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            var response = new
            {
                message = ex.Message,
                statusCode = (int)statusCode
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
