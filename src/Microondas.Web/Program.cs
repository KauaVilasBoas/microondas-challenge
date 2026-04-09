using FluentValidation;
using Microondas.Application.Commands.Behaviors;
using Microondas.Application.EventHandlers;
using Microondas.Infrastructure;
using Microondas.Infrastructure.Logging;
using Microondas.Web.Filters;
using Microondas.Web.Hubs;
using Microondas.Web.Middleware;
using Microondas.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// ── Database & domain infrastructure ─────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddInfrastructure(connectionString);

// ── CQRS pipeline (MediatR) ───────────────────────────────────────────────────
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
builder.Services.AddScoped<IHeatingHubNotifier, HeatingHubNotifier>();

// ── Background timer (ticks the heating session every second) ─────────────────
builder.Services.AddHostedService<Microondas.Workers.HeatingTimerService>();

// ── Session — stores the Bearer token returned by the REST API ────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".Microondas.Session";
});

builder.Services.AddHttpContextAccessor();

// ── API authentication client (used only for /api/auth/login) ─────────────────
builder.Services.AddScoped<ITokenStore, SessionTokenStore>();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("'ApiSettings:BaseUrl' is not configured.");

builder.Services.AddHttpClient<IApiAuthService, ApiAuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
});

// ── Auth filter (applied per-controller via [TypeFilter<RequireApiAuthFilter>]) ─
builder.Services.AddScoped<RequireApiAuthFilter>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<HeatingHub>("/heatingHub");

app.Run();
