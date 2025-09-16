using Microsoft.EntityFrameworkCore;
using MySvelteApp.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MySvelteApp.Server.Services;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

// 1) Define CORS policy name
const string WebsiteClientOrigin = "website_client";

// 2) Configure CORS to allow your actual client URL
builder.Services.AddCors(options =>
{
    options.AddPolicy(WebsiteClientOrigin, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000", "http://web:3000", "http://localhost:5173") // Allow multiple origins
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Allow credentials to be sent
    });
});

// Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "your-issuer",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "your-secret-key-here"))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

builder.Services.AddControllers(options =>
{
    // Require authorization for all controllers by default
    // Remove this if you want to opt-in to authorization instead
    // options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MySvelteApp.Server", Version = "v1" });

    // 1) Define the Bearer auth scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,          // HTTP auth
        Scheme = "bearer",                       // Bearer scheme
        BearerFormat = "JWT",                    // Optional, for docs
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer <token>'"
    });

    // 2) Apply it globally as a requirement
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
builder.Services.AddHttpClient();

var promtailUrl = builder.Configuration["LOKI_PUSH_URL"] ?? "http://localhost:3101/loki/api/v1/push";
var apiServiceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? "mysvelteapp-api";
var environmentName = builder.Environment.EnvironmentName ?? "Development";

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .Enrich.WithProperty("service", apiServiceName)
        .Enrich.WithProperty("env", environmentName.ToLowerInvariant())
        .WriteTo.Console()
        .WriteTo.GrafanaLoki(promtailUrl);
});

var serviceName = apiServiceName;
var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4318/v1/traces";
var otlpProtocol = builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"] ?? "http/protobuf";

builder.Services.AddOpenTelemetry().WithTracing(tracing =>
{
    tracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddAspNetCoreInstrumentation(options =>
        {
            options.RecordException = true;
        })
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(otlpEndpoint);
            options.Protocol = string.Equals(otlpProtocol, "grpc", StringComparison.OrdinalIgnoreCase)
                ? OtlpExportProtocol.Grpc
                : OtlpExportProtocol.HttpProtobuf;
        });
});

// Add Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("MySvelteAppDb")); // Using in-memory database for simplicity

// Register services
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// 3) Register CORS middleware before controllers
app.UseCors(WebsiteClientOrigin);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
