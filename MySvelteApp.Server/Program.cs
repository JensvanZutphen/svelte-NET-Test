using Microsoft.EntityFrameworkCore;
using MySvelteApp.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MySvelteApp.Server.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) Define CORS policy name
const string WebsiteClientOrigin = "website_client";

// 2) Configure CORS to allow your actual client URL
builder.Services.AddCors(options =>
{
    options.AddPolicy(WebsiteClientOrigin, policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
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

builder.Services.AddAuthorization(options =>
{
    // Make authentication required by default for all endpoints
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

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

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
