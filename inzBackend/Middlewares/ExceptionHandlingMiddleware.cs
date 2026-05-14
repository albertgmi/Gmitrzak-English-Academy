using inzBackend.Exceptions;
using System.Net;
using System.Text.Json;

namespace inzBackend.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadRequestException badRequestException)
            {
                await HandleExceptionAsync(context, badRequestException, HttpStatusCode.BadRequest);
            }
            catch (NotFoundException notFoundException)
            {
                await HandleExceptionAsync(context, notFoundException, HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Wystąpił nieobsłużony wyjątek");

                await HandleExceptionAsync(context, e, HttpStatusCode.InternalServerError, "Wystąpił nieoczekiwany błąd serwera.");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, string? customMessage = null)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = context.Response.StatusCode,
                message = customMessage ?? exception.Message,
                timestamp = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
