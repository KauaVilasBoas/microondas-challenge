using Microondas.Application.Commands.Behaviors;
using Microondas.Domain.Contracts.Commands;
using Microondas.Domain.Contracts.Repositories;
using Microondas.Domain.Programs;
using Microondas.SharedKernel;

namespace Microondas.Application.Commands.Programs.DeleteCustomProgram;

public sealed class DeleteCustomProgramCommandHandler : ICommandHandler<DeleteCustomProgramCommand>
{
    private readonly IHeatingProgramRepository _programRepository;
    private readonly DomainEventCollector _collector;

    public DeleteCustomProgramCommandHandler(
        IHeatingProgramRepository programRepository,
        DomainEventCollector collector)
    {
        _programRepository = programRepository;
        _collector = collector;
    }

    public async Task<Result> Handle(DeleteCustomProgramCommand command, CancellationToken cancellationToken)
    {
        var program = await _programRepository.GetByIdAsync<HeatingProgram>(command.ProgramId, cancellationToken);

        if (program is null)
            return Error.NotFound("HeatingProgram.NotFound", $"Programa '{command.ProgramId}' não encontrado.");

        var deleteResult = program.Delete();
        if (deleteResult.IsFailure) return deleteResult;

        await _programRepository.RemoveAsync(program, cancellationToken);
        await _programRepository.SaveChangesAsync(cancellationToken);

        _collector.Collect(program);

        return Result.Success();
    }
}