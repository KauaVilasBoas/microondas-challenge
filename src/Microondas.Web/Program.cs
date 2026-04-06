using FluentValidation;
using Microondas.Application.Commands.Behaviors;
using Microondas.Application.EventHandlers;
using Microondas.Infrastructure;
using Microondas.Web.Hubs;
using Microondas.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddInfrastructure(connectionString);

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

builder.Services.AddHostedService<Microondas.Workers.HeatingTimerService>();

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
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<HeatingHub>("/heatingHub");

app.Run();
