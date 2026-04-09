using Microondas.Domain.Repositories;
using Microondas.Domain.Programs;
using Microsoft.EntityFrameworkCore;

namespace Microondas.Infrastructure.Persistence.Repositories;

public sealed class HeatingProgramRepository : IHeatingProgramRepository
{
    private readonly MicroondasDbContext _context;

    public HeatingProgramRepository(MicroondasDbContext context) => _context = context;

    public async Task<IReadOnlyList<TProgram>> GetAllAsync<TProgram>(CancellationToken cancellationToken = default)
        where TProgram : class
    {
        if (typeof(TProgram) != typeof(HeatingProgram))
            return [];

        var programs = await _context.HeatingPrograms
            .Where(p => p.Type == ProgramType.Custom)
            .ToListAsync(cancellationToken);

        return (IReadOnlyList<TProgram>)programs;
    }

    public async Task<TProgram?> GetByIdAsync<TProgram>(Guid id, CancellationToken cancellationToken = default)
        where TProgram : class
    {
        if (typeof(TProgram) != typeof(HeatingProgram)) return null;

        var program = await _context.HeatingPrograms
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return program as TProgram;
    }

    public async Task<bool> ExistsByCharacterAsync(char character, CancellationToken cancellationToken = default) =>
        await _context.HeatingPrograms
            .AnyAsync(p => p.Character.Value == character, cancellationToken);

    public async Task<bool> ExistsByCharacterExcludingAsync(char character, Guid excludeId,
        CancellationToken cancellationToken = default) =>
        await _context.HeatingPrograms
            .AnyAsync(p => p.Character.Value == character && p.Id != excludeId, cancellationToken);

    public async Task AddAsync<TProgram>(TProgram program, CancellationToken cancellationToken = default)
        where TProgram : class
    {
        if (program is HeatingProgram heatingProgram)
            await _context.HeatingPrograms.AddAsync(heatingProgram, cancellationToken);
    }

    public async Task RemoveAsync<TProgram>(TProgram program, CancellationToken cancellationToken = default)
        where TProgram : class
    {
        if (program is HeatingProgram heatingProgram)
            _context.HeatingPrograms.Remove(heatingProgram);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}