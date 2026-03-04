using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Serilog;
using WalletManagement.Application.DependencyResolvers;
using WalletManagement.InnerInfrastructure.DependencyResolvers;
using WalletManagement.Persistence.DependencyResolvers;
using WalletManagement.WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddRepositoryServices();
builder.Services.AddManagerServices();
builder.Services.AddMapperService();
builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddLoggerService(builder.Configuration);
builder.Host.UseSerilog();
builder.Services.AddValidatorServices();
builder.Services.AddFluentValidationAutoValidation(); 

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Wallet Management System API",
        Version = "v1",
        Description = "4ARC Yaz�l�m Staj Projesi - C�zdan Y�netim Sistemi API Dok�mantasyonu"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
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

var corsOrigins = (Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")
    ?? builder.Configuration["CorsSettings:AllowedOrigins"]
    ?? "http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
