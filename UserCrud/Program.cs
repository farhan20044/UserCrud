using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UserCrud.Helpers;
using UserCrud.Models;
using UserCrud.Models.Dto;
using UserCrud.Services;
using UserCrud.Services.Interfaces;
using UserCrud.Repository;
using UserCrud.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services to the container
builder.Services.AddControllers();

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<UserdbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddDbContext<UserdbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger registration
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

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Register your user service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Add Generic Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Enable Swagger UI in both development and production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User CRUD API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");


app.UseHttpsRedirection();

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
