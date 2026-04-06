using Microsoft.EntityFrameworkCore;
using Microondas.Domain.Programs;
using Microondas.Infrastructure.Persistence.Configurations;

namespace Microondas.Infrastructure.Persistence;

public sealed class MicroondasDbContext : DbContext
{
    public MicroondasDbContext(DbContextOptions<MicroondasDbContext> options) : base(options) { }

    public DbSet<HeatingProgram> HeatingPrograms => Set<HeatingProgram>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new HeatingProgramConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
