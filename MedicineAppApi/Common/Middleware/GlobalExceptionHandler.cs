using System.Net;
using System.Text.Json;
using MedicineAppApi.Common.Exceptions;
using MedicineAppApi.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MedicineAppApi.Common.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;
            var errorResponse = CreateErrorResponse(exception, traceId);

            _logger.LogError(exception, "An error occurred. TraceId: {TraceId}, Error: {Error}", traceId, exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)errorResponse.StatusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }

        private ErrorResponse CreateErrorResponse(Exception exception, string traceId)
        {
            return exception switch
            {
                ValidationException validationException => new ErrorResponse(
                    validationException.Message,
                    validationException.ErrorCode,
                    validationException.StatusCode,
                    validationException.Errors,
                    validationException.AdditionalData)
                {
                    TraceId = traceId
                },

                AppException appException => new ErrorResponse(
                    appException.Message,
                    appException.ErrorCode,
                    appException.StatusCode,
                    appException.AdditionalData)
                {
                    TraceId = traceId
                },

                UnauthorizedAccessException => new ErrorResponse(
                    "Access denied. You are not authorized to perform this action.",
                    "UNAUTHORIZED_ACCESS",
                    HttpStatusCode.Unauthorized)
                {
                    TraceId = traceId
                },

                ArgumentNullException => new ErrorResponse(
                    exception.Message,
                    "NULL_ARGUMENT",
                    HttpStatusCode.BadRequest)
                {
                    TraceId = traceId
                },

                ArgumentException => new ErrorResponse(
                    exception.Message,
                    "INVALID_ARGUMENT",
                    HttpStatusCode.BadRequest)
                {
                    TraceId = traceId
                },

                InvalidOperationException => new ErrorResponse(
                    exception.Message,
                    "INVALID_OPERATION",
                    HttpStatusCode.BadRequest)
                {
                    TraceId = traceId
                },

                KeyNotFoundException => new ErrorResponse(
                    exception.Message,
                    "KEY_NOT_FOUND",
                    HttpStatusCode.NotFound)
                {
                    TraceId = traceId
                },

                _ => new ErrorResponse(
                    _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                    "INTERNAL_SERVER_ERROR",
                    HttpStatusCode.InternalServerError,
                    _environment.IsDevelopment() ? new { StackTrace = exception.StackTrace } : null)
                {
                    TraceId = traceId
                }
            };
        }
    }
}
