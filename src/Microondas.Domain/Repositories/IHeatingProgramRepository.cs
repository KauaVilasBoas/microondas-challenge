namespace Microondas.Domain.Repositories;

public interface IHeatingProgramRepository
{
    Task<IReadOnlyList<TProgram>> GetAllAsync<TProgram>(CancellationToken cancellationToken = default)
        where TProgram : class;

    Task<TProgram?> GetByIdAsync<TProgram>(Guid id, CancellationToken cancellationToken = default)
        where TProgram : class;

    Task<bool> ExistsByCharacterAsync(char character, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCharacterExcludingAsync(char character, Guid excludeId,
        CancellationToken cancellationToken = default);

    Task AddAsync<TProgram>(TProgram program, CancellationToken cancellationToken = default)
        where TProgram : class;

    Task RemoveAsync<TProgram>(TProgram program, CancellationToken cancellationToken = default)
        where TProgram : class;

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
