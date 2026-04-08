using Microondas.Application.Commands.Behaviors;
using Microondas.Domain.Contracts.Commands;
using Microondas.Domain.Contracts.Repositories;
using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Programs;
using Microondas.SharedKernel;

namespace Microondas.Application.Commands.Programs.CreateCustomProgram;

public sealed class CreateCustomProgramCommandHandler : ICommandHandler<CreateCustomProgramCommand>
{
    private readonly IHeatingProgramRepository _programRepository;
    private readonly DomainEventCollector _collector;

    public CreateCustomProgramCommandHandler(
        IHeatingProgramRepository programRepository,
        DomainEventCollector collector)
    {
        _programRepository = programRepository;
        _collector = collector;
    }

    public async Task<Result> Handle(CreateCustomProgramCommand command, CancellationToken cancellationToken)
    {
        var charIsReserved = PredefinedProgramSeed.ReservedCharacters.Contains(command.HeatingChar);
        if (charIsReserved)
            return Error.Conflict("HeatingCharacter.Reserved",
                $"O caractere '{command.HeatingChar}' já é utilizado por um programa pré-definido.");

        var charExists = await _programRepository.ExistsByCharacterAsync(command.HeatingChar, cancellationToken);
        if (charExists)
            return Error.Conflict("HeatingCharacter.Duplicate",
                $"O caractere '{command.HeatingChar}' já está em uso por outro programa.");

        var programResult = HeatingProgram.CreateCustom(
            command.Name,
            command.Food,
            command.TimeInSeconds,
            command.Power,
            command.HeatingChar,
            command.Instructions);

        if (programResult.IsFailure) return Result.Failure(programResult.Error);

        await _programRepository.AddAsync(programResult.Value, cancellationToken);
        await _programRepository.SaveChangesAsync(cancellationToken);

        _collector.Collect(programResult.Value);

        return Result.Success();
    }
}
