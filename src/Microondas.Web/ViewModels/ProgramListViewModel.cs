using Microondas.Application.ReadModels.Programs;

namespace Microondas.Web.ViewModels;

public sealed class ProgramListViewModel
{
    public IReadOnlyList<ProgramReadModel> Programs { get; init; } = [];
    public string? ErrorMessage { get; init; }
}