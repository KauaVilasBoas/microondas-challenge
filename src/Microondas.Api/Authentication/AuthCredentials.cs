namespace Microondas.Api.Authentication;

public sealed class AuthCredentials
{
    public string Username { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
}
