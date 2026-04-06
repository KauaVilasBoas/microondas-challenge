using System.ComponentModel.DataAnnotations;

namespace Microondas.Web.ViewModels;

public sealed class HeatingViewModel
{
    [Range(1, 120, ErrorMessage = "Tempo deve estar entre 1 e 120 segundos.")]
    public int? TimeInSeconds { get; set; }

    [Range(1, 10, ErrorMessage = "Potência deve estar entre 1 e 10.")]
    public int? PowerLevel { get; set; }

    public char? HeatingChar { get; set; }
    public string? SelectedProgramId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
