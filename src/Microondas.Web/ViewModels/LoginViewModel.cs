using System.ComponentModel.DataAnnotations;

namespace Microondas.Web.ViewModels;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Usuário é obrigatório.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}