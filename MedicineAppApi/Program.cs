using MedicineAppApi.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application services using extension method
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add global exception handling middleware (must be early in pipeline)
app.UseGlobalExceptionHandler();

// Add validation error handling middleware
app.UseValidationErrorHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseApplicationEndpoints();

app.Run();
