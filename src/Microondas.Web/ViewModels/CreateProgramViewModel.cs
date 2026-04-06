using System.ComponentModel.DataAnnotations;

namespace Microondas.Web.ViewModels;

public sealed class CreateProgramViewModel
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Alimento é obrigatório.")]
    [MaxLength(100)]
    public string Food { get; set; } = string.Empty;

    [Required]
    [Range(1, 120, ErrorMessage = "Tempo deve estar entre 1 e 120 segundos.")]
    public int TimeInSeconds { get; set; }

    [Required]
    [Range(1, 10, ErrorMessage = "Potência deve estar entre 1 e 10.")]
    public int Power { get; set; }

    [Required(ErrorMessage = "Caractere de aquecimento é obrigatório.")]
    public char HeatingChar { get; set; }

    [MaxLength(500)]
    public string? Instructions { get; set; }

    public string? ErrorMessage { get; set; }
}
