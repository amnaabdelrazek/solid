//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using Solid.Api.Database;
//using Solid.Api.Database.Repositories;
//using Solid.Api.Features.Auth;
//using Solid.Api.Features.Content;
//using Solid.Api.Features.Groups;
//using Solid.Api.Features.Lookups;
//using Solid.Api.Features.Notifications;
//using Solid.Api.Features.Payments;
//using Solid.Api.Features.Recommendations;
//using Solid.Api.Features.Sessions;
//using Solid.Api.Features.Settings;
//using Solid.Api.Features.Users;
//using Solid.Api.Infrastructure.Auth;
//using Solid.Api.Infrastructure.Jitsi;
//using Solid.Api.Infrastructure.Sms;
//using Solid.Api.Seeds;
//using Stripe;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//#region Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter: Bearer {your token}"
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});
//#endregion

//#region Core Services
//builder.Services.AddMemoryCache();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddSignalR();
//builder.Services.AddHttpClient();
//#endregion

////#region Infobip Settings (IMPORTANT)
////builder.Services.Configure<InfobipSettings>(
////    builder.Configuration.GetSection("Infobip"));
////#endregion

//#region SMS + OTP
////builder.Services.AddHttpClient<ISmsSender, InfobipSmsSender>((sp, client) =>
////{
////    var config = sp.GetRequiredService<IConfiguration>();

////    var baseUrl = config["Sms:Infobip:BaseUrl"];
////    var apiKey = config["Sms:Infobip:ApiKey"];

////    if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
////        throw new InvalidOperationException("Infobip config is missing.");

////    client.BaseAddress = new Uri(baseUrl);

////    client.DefaultRequestHeaders.Authorization =
////        new System.Net.Http.Headers.AuthenticationHeaderValue("App", apiKey);
////});

////builder.Services.AddScoped<IOtpService, OtpService>();

//builder.Services.AddMemoryCache();

//#region SMS + OTP (Infobip 2FA)
//builder.Services.AddHttpClient<IOtpService, OtpService>((sp, client) =>
//{
//    var config = sp.GetRequiredService<IConfiguration>();

//    var baseUrl = config["Sms:Infobip:BaseUrl"];   // ✅ بدل Infobip:BaseUrl
//    var apiKey = config["Sms:Infobip:ApiKey"];      // ✅

//    if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
//        throw new InvalidOperationException("Infobip config missing");

//    client.BaseAddress = new Uri(baseUrl);
//    client.DefaultRequestHeaders.Add("Authorization", $"App {apiKey}");
//});
//#endregion
////builder.Services.AddScoped<IAuthService, AuthService>();
//#endregion

//#region Database
//builder.Services.AddDbContext<SolidDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection")));
//#endregion

//#region Stripe
//StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
//#endregion

//#region Dependency Injection
//builder.Services.AddScoped<IAuthContext, JwtAuthContext>();
//builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
//builder.Services.AddScoped<IJwtTokenRevocationService, JwtTokenRevocationService>();

//builder.Services.AddScoped<IAuthRepository, AuthRepository>();
//builder.Services.AddScoped<ICacheRepository, CacheRepository>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
//builder.Services.AddScoped<ILookupRepository, LookupRepository>();
//builder.Services.AddScoped<IGroupRepository, GroupRepository>();
//builder.Services.AddScoped<ISessionRepository, SessionRepository>();
//builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
//builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();

//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<INotificationService, NotificationService>();

//builder.Services.AddScoped<IJaasTokenService, JaasTokenService>();
//#endregion

//#region JWT Auth
//var jwtKey = builder.Configuration["Jwt:Key"]
//    ?? throw new InvalidOperationException("Jwt:Key is required.");

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(jwtKey)),

//            ValidateIssuer = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],

//            ValidateAudience = true,
//            ValidAudience = builder.Configuration["Jwt:Audience"],

//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.FromMinutes(1)
//        };

//        options.Events = new JwtBearerEvents
//        {
//            OnTokenValidated = async context =>
//            {
//                var authHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
//                var token = authHeader?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

//                if (string.IsNullOrWhiteSpace(token))
//                {
//                    context.Fail("Token is missing.");
//                    return;
//                }

//                var revocationService = context.HttpContext.RequestServices
//                    .GetRequiredService<IJwtTokenRevocationService>();

//                if (await revocationService.IsRevokedAsync(token))
//                    context.Fail("Token has been revoked.");
//            }
//        };
//    });

//builder.Services.AddAuthorization();
//#endregion

//var app = builder.Build();

//#region Seeder
//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<SolidDbContext>();
//    await DatabaseSeeder.SeedAsync(dbContext);
//}
//#endregion

//#region Swagger UI
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//#endregion

//app.UseAuthentication();
//app.UseAuthorization();

//#region API Routes
//var api = app.MapGroup("/api");

//api.MapContentSlice();
//api.MapLookupSlice();
//api.MapAuthSlice();

//var protectedApi = api.MapGroup("").RequireAuthorization();
//protectedApi.MapUserSlice();
//protectedApi.MapGroupSlice();
//protectedApi.MapSessionSlice();
//protectedApi.MapPaymentSlice();
//protectedApi.MapRecommendationSlice();
//protectedApi.MapSettingsSlice();
//#endregion

//#region SignalR
//app.MapHub<NotificationsHub>("/hubs/notifications")
//   .RequireAuthorization();
//#endregion

//app.Run();


using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
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
using Solid.Api.Infrastructure;
using Solid.Api.Infrastructure.Sms;
using Solid.Api.Seeds;
using Stripe;
using System.Text;
using System.Text.Json;

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
#endregion

#region Core Services
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();
#endregion

#region SMS + OTP (Infobip 2FA)
builder.Services.AddHttpClient<IOtpService, OtpService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var baseUrl = config["Sms:Infobip:BaseUrl"];
    var apiKey = config["Sms:Infobip:ApiKey"];

    if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
        throw new InvalidOperationException("Infobip config missing");

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"App {apiKey}");
});
#endregion

#region Firebase (Cloud Messaging push notifications)
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];
var firebaseClientEmail = builder.Configuration["Firebase:ClientEmail"];
var firebasePrivateKey = builder.Configuration["Firebase:PrivateKey"];

if (!string.IsNullOrWhiteSpace(firebaseProjectId) &&
    !string.IsNullOrWhiteSpace(firebaseClientEmail) &&
    !string.IsNullOrWhiteSpace(firebasePrivateKey) &&
    FirebaseApp.DefaultInstance is null)
{
    var serviceAccountJson = JsonSerializer.Serialize(new
    {
        type = "service_account",
        project_id = firebaseProjectId,
        private_key = firebasePrivateKey,
        client_email = firebaseClientEmail,
        token_uri = "https://oauth2.googleapis.com/token"
    });

    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromJson(serviceAccountJson)
    });
}

builder.Services.AddSingleton<IPushNotificationService, PushNotificationService>();
#endregion

#region Database
builder.Services.AddDbContext<SolidDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
#endregion

#region Dependency Injection
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
builder.Services.AddScoped<INotificationService, NotificationService>();

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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

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
                var authHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
                var token = authHeader?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrWhiteSpace(token))
                {
                    context.Fail("Token is missing.");
                    return;
                }

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

#region Seeder
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SolidDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext);
}
#endregion

#region Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

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

#region SignalR
app.MapHub<NotificationsHub>("/hubs/notifications")
   .RequireAuthorization();
#endregion

app.Run();
