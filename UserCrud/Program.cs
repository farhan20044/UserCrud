using Microsoft.OpenApi.Models;
using UserCrud.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Swagger registration with custom info
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User CRUD API",
        Version = "v1",
        Description = "A simple ASP.NET Core Web API for managing users",
        Contact = new OpenApiContact
        {
            Name = "Farhan",
            Email = "farhan.jameel@hazelsoft.com"
        }
    });
});

// Register your user service
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Enable Swagger UI in both development and production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User CRUD API v1");
    c.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
