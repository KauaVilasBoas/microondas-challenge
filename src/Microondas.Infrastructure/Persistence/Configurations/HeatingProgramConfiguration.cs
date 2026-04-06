using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microondas.Domain.Programs;

namespace Microondas.Infrastructure.Persistence.Configurations;

public sealed class HeatingProgramConfiguration : IEntityTypeConfiguration<HeatingProgram>
{
    public void Configure(EntityTypeBuilder<HeatingProgram> builder)
    {
        builder.ToTable("HeatingPrograms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Food, food =>
        {
            food.Property(f => f.Value)
                .HasColumnName("Food")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Time, time =>
        {
            time.Property(t => t.TotalSeconds)
                .HasColumnName("TimeInSeconds")
                .IsRequired();
        });

        builder.OwnsOne(x => x.Power, power =>
        {
            power.Property(p => p.Value)
                .HasColumnName("Power")
                .IsRequired();
        });

        builder.OwnsOne(x => x.Character, character =>
        {
            character.Property(c => c.Value)
                .HasColumnName("HeatingChar")
                .HasMaxLength(1)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Instructions, instructions =>
        {
            instructions.Property(i => i.Value)
                .HasColumnName("Instructions")
                .HasMaxLength(500)
                .IsRequired(false);
        });
    }
}
