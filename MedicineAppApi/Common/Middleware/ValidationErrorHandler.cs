using System.Net;
using System.Text.Json;
using MedicineAppApi.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MedicineAppApi.Common.Middleware
{
    public class ValidationErrorHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationErrorHandler> _logger;

        public ValidationErrorHandler(RequestDelegate next, ILogger<ValidationErrorHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest && !context.Response.HasStarted)
            {
                await HandleValidationErrorsAsync(context);
            }
        }

        private async Task HandleValidationErrorsAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                // Read the original response
                memoryStream.Position = 0;
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                // Check if it's a validation error response
                if (responseBody.Contains("validation") || responseBody.Contains("Validation"))
                {
                    var fieldErrors = ExtractFieldErrors(responseBody);
                    var errorResponse = new ValidationErrorResponse("One or more validation errors occurred.", fieldErrors)
                    {
                        TraceId = context.TraceIdentifier
                    };

                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };

                    var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);

                    context.Response.Body = originalBodyStream;
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    await context.Response.WriteAsync(jsonResponse);
                }
                else
                {
                    // Restore original response
                    context.Response.Body = originalBodyStream;
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling validation errors");
                context.Response.Body = originalBodyStream;
            }
        }

        private Dictionary<string, List<string>> ExtractFieldErrors(string responseBody)
        {
            var fieldErrors = new Dictionary<string, List<string>>();

            try
            {
                // Try to parse the response as JSON
                var jsonDoc = JsonDocument.Parse(responseBody);
                if (jsonDoc.RootElement.TryGetProperty("errors", out var errorsElement))
                {
                    foreach (var error in errorsElement.EnumerateObject())
                    {
                        var fieldName = error.Name;
                        var errorMessages = new List<string>();

                        if (error.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var message in error.Value.EnumerateArray())
                            {
                                errorMessages.Add(message.GetString() ?? "");
                            }
                        }
                        else
                        {
                            errorMessages.Add(error.Value.GetString() ?? "");
                        }

                        fieldErrors[fieldName] = errorMessages;
                    }
                }
            }
            catch
            {
                // If parsing fails, create a generic error
                fieldErrors["general"] = new List<string> { "Validation failed" };
            }

            return fieldErrors;
        }
    }
}
