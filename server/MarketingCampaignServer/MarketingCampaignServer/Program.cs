using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MarketingCampaignServer.Data;
using MarketingCampaignServer.Helpers;
using MarketingCampaignServer.Services;
using MarketingCampaignServer.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// 1️⃣  Read JWT configuration
// -----------------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// -----------------------------
// 2️⃣  Register Helper & Services (Transient)
// -----------------------------
builder.Services.AddTransient<JwtTokenHelper>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ICampaignService, CampaignService>();
builder.Services.AddTransient<ILeadService, LeadService>();
builder.Services.AddTransient<IEngagementMetricService, EngagementMetricService>();
builder.Services.AddTransient<ICampaignPerformanceSnapshotService, CampaignPerformanceSnapshotService>();
builder.Services.AddTransient<IBulkUploadService, BulkUploadService>();
builder.Services.AddTransient<SegmentAssignmentHelper>();



// -----------------------------
// 3️⃣  Register DbContext (MySQL)
// -----------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// -----------------------------
// 4️⃣  Register AutoMapper (Transient)
// -----------------------------
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// -----------------------------
// 5️⃣  JWT Authentication Setup
// -----------------------------
var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

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
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // exact expiry control
    };

    // Custom error handling for expired or invalid tokens
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Session expired. Please log in again.\"}");
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Unauthorized access. Please provide a valid token.\"}");
            }
            return Task.CompletedTask;
        }
    };
});

// -----------------------------
// 6️⃣  Swagger Setup (Bearer Default)
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Marketing Campaign API",
        Version = "v1",
        Description = "Web API for Marketing Campaign Management System"
    });

    // Enable JWT Auth in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token (e.g. Bearer eyJhbGciOi...)"
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
            new string[] {}
        }
    });
});

// -----------------------------
// 7️⃣  Add CORS and Controllers
// -----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();

// -----------------------------
// 8️⃣  Build and Configure Pipeline
// -----------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
