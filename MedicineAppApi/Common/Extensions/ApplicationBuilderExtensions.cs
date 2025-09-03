using MedicineAppApi.Common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace MedicineAppApi.Common.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandler>();
        }

        public static IApplicationBuilder UseValidationErrorHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ValidationErrorHandler>();
        }

        public static IApplicationBuilder UseApplicationEndpoints(this IApplicationBuilder app)
        {
            return app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
