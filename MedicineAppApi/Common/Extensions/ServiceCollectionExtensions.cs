using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MedicineAppApi.Data;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;
using MedicineAppApi.Services;
using MedicineAppApi.Mapping;

namespace MedicineAppApi.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Entity Framework
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Add Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "YourSuperSecretKey123!@#"))
                    };
                });

            // Add AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // Add Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IMedicineRepository, MedicineRepository>();
            services.AddScoped<ISupplierMedicineRepository, SupplierMedicineRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IInvoiceItemRepository, InvoiceItemRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<IPurchaseItemRepository, PurchaseItemRepository>();

            // Add Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IStockMovementService, StockMovementService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IReportsService, ReportsService>();

            return services;
        }
    }
}
