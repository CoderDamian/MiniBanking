using Microsoft.AspNetCore.Mvc;
using Mini_Banking.Domain.Exceptions;

namespace Mini_Banking.APIRest.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate nextMiddleware, ILogger<GlobalExceptionMiddleware> logger)
        {
            this._next = nextMiddleware;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Do something before the next middleware

            try
            {
                await _next(context);   // Call the next middleware in the pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception ocurred...");
                await ValidateAsync(ex, context);
            }
        }

        private static async Task ValidateAsync(Exception ex, HttpContext context)
        {
            var statusCode = (ex) switch
            {
                DomainException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new ProblemDetails
            {
                Title = ex.Message,
                Status = statusCode
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
