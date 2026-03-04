using System.Security.Claims;
using DiscountsSystem.Application;
using DiscountsSystem.Application.DTOs.Auth;
using DiscountsSystem.Api.Middleware;
using DiscountsSystem.Infrastructure;
using DiscountsSystem.Infrastructure.Persistence.Identity.Seeding;
using DiscountsSystem.Infrastructure.Persistence.Seeding;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using Asp.Versioning;
using DiscountsSystem.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

// 1. Services Configuration
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(DiscountsSystem.Application.DependencyInjection).Assembly);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));

var cfg = builder.Configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>()
          ?? throw new InvalidOperationException("JwtConfiguration missing.");

// Auth Configuration
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = cfg.Issuer,
            ValidAudience = cfg.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg.Key)),
            ClockSkew = TimeSpan.Zero,

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization();

// Health Check
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DiscountsDbContext>(name: "database");

// API Versioning
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DiscountsSystem API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Value Token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// HTTP Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await IdentitySeeder.SeedAsync(app.Services);
    await SettingsSeeder.SeedAsync(app.Services);
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapGet("/__endpoints", (IEnumerable<EndpointDataSource> sources) =>
{
    var routes = sources
        .SelectMany(s => s.Endpoints)
        .OfType<RouteEndpoint>()
        .Select(e => e.RoutePattern.RawText)
        .Distinct()
        .OrderBy(x => x)
        .ToList();

    return Results.Ok(routes);
}).AllowAnonymous();

// Health Mapping - http://localhost:5118/health
app.MapHealthChecks("/health").AllowAnonymous();

app.MapControllers();

app.Run();
