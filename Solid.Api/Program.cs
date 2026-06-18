using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Solid.Api.Database;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Auth;
using Solid.Api.Features.Content;
using Solid.Api.Features.Groups;
using Solid.Api.Features.Lookups;
using Solid.Api.Features.Notifications;
using Solid.Api.Features.Payments;
using Solid.Api.Features.Recommendations;
using Solid.Api.Features.Sessions;
using Solid.Api.Features.Settings;
using Solid.Api.Features.Users;
using Solid.Api.Infrastructure.Auth;
using Solid.Api.Infrastructure.Jitsi;
using Solid.Api.Infrastructure.Sms;
using Solid.Api.Seeds;
using Stripe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
#endregion

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

// SMS provider
if (builder.Configuration["Sms:Provider"]?.Equals("Twilio", StringComparison.OrdinalIgnoreCase) == true)
    builder.Services.AddScoped<ISmsSender, TwilioVerifySender>();
else
    builder.Services.AddScoped<ISmsSender, LocalOtpSender>();

// Database
builder.Services.AddDbContext<SolidDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Stripe global config
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

#region DI
builder.Services.AddScoped<IAuthContext, JwtAuthContext>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IJwtTokenRevocationService, JwtTokenRevocationService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ICacheRepository, CacheRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// ✅ JaaS token service
builder.Services.AddScoped<IJaasTokenService, JaasTokenService>();
#endregion

#region JWT Auth
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is required.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var authorization = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
                var token = authorization?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                if (string.IsNullOrWhiteSpace(token)) { context.Fail("Token is missing."); return; }

                var revocationService = context.HttpContext.RequestServices
                    .GetRequiredService<IJwtTokenRevocationService>();
                if (await revocationService.IsRevokedAsync(token))
                    context.Fail("Token has been revoked.");
            }
        };
    });

builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

// ✅ Seeder only in Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SolidDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

#region API Routes
var api = app.MapGroup("/api");

api.MapContentSlice();
api.MapLookupSlice();
api.MapAuthSlice();

var protectedApi = api.MapGroup("").RequireAuthorization();
protectedApi.MapUserSlice();
protectedApi.MapGroupSlice();
protectedApi.MapSessionSlice();
protectedApi.MapPaymentSlice();
protectedApi.MapRecommendationSlice();
protectedApi.MapSettingsSlice();
#endregion

app.MapHub<NotificationsHub>("/hubs/notifications").RequireAuthorization();

app.Run();
