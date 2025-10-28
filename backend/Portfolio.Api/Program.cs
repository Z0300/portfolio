using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api;
using Portfolio.Api.Endpoints;
using Scalar.AspNetCore;
using Microsoft.OpenApi.Models;



DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
var jwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET");
var jwtAudience = Environment.GetEnvironmentVariable("SUPABASE_JWT_AUDIENCE");
var supabaseServiceRoleKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE_KEY");
var adminUserId = Environment.GetEnvironmentVariable("SUPABASE_ADMIN_USER_ID");
var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");

var AllowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");


builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpoints();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{supabaseUrl}/auth/v1",
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(AllowedOrigins!.Split(','))
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Title = "Portfolio API";
        options.Theme = ScalarTheme.Mars;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.MapEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowFrontend");

app.Run();

