using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microondas.Api.Authentication;
using Microondas.Api.Middleware;
using Microondas.Application.Commands.Behaviors;
using Microondas.Application.EventHandlers;
using Microondas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddInfrastructure(connectionString);

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? new JwtSettings { Secret = "MicroondasDefaultSecretKey2024!!", Issuer = "Microondas.Api", Audience = "Microondas.Web" };

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<JwtTokenGenerator>();

var credentials = builder.Configuration.GetSection("AuthCredentials").Get<AuthCredentials>()
    ?? new AuthCredentials { Username = "admin", HashedPassword = Microondas.Infrastructure.Services.PasswordHasher.HashSha256("admin") };
builder.Services.AddSingleton(credentials);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Microondas.Application.Commands.Heating.StartHeating.StartHeatingCommandHandler).Assembly,
        typeof(Microondas.Application.ReadModels.Heating.GetHeatingStatusQueryHandler).Assembly,
        typeof(Microondas.Application.EventHandlers.Heating.HeatingStartedEventHandler).Assembly);

    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(DomainEventDispatchBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(
    typeof(Microondas.Application.Commands.Heating.StartHeating.StartHeatingCommandValidator).Assembly);

builder.Services.AddScoped<DomainEventCollector>();

builder.Services.AddSingleton<IHeatingHubNotifier, Microondas.Api.Notifications.NoOpHeatingHubNotifier>();

builder.Services.AddHostedService<Microondas.Workers.HeatingTimerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
