var builder = WebApplication.CreateBuilder(args);

// 1) Define CORS policy name
const string WebsiteClientOrigin = "website_client";

// 2) Configure CORS to allow your actual client URL
builder.Services.AddCors(options =>
{
    options.AddPolicy(WebsiteClientOrigin, policy =>
    {
        // Add both prod and dev origins here
        policy.WithOrigins(
                "https://your-production-site.com",
                "http://localhost:3000"      // ← add this
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// 3) Register CORS middleware before controllers
app.UseCors(WebsiteClientOrigin);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
