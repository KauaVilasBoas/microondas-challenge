using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microondas.Domain.Contracts.Repositories;
using Microondas.Domain.Services;
using Microondas.Infrastructure.Logging;
using Microondas.Infrastructure.Persistence;
using Microondas.Infrastructure.Persistence.Repositories;
using Microondas.Infrastructure.Services;
using Microondas.SharedKernel;

namespace Microondas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<MicroondasDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IHeatingProgramRepository, HeatingProgramRepository>();
        services.AddSingleton<HeatingSessionHolder>();
        services.AddSingleton<IHeatingOutputRenderer, HeatingOutputRenderer>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<FileExceptionLogger>();

        return services;
    }
}
