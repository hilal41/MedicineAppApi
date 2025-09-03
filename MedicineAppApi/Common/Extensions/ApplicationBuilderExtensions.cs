namespace MedicineAppApi.Common.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        public static IEndpointRouteBuilder UseApplicationEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
            return endpoints;
        }
    }
}
