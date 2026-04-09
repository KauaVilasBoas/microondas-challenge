using Microondas.SharedKernel;

namespace Microondas.Web.Services;

/// <summary>
/// Calls <c>POST /api/auth/login</c> on the REST API and returns the JWT Bearer token.
/// Credentials are validated server-side (SHA-256 hashing happens in the API).
/// </summary>
public sealed class ApiAuthService : IApiAuthService
{
    private readonly HttpClient _http;
    private readonly ILogger<ApiAuthService> _logger;

    public ApiAuthService(HttpClient http, ILogger<ApiAuthService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<Result<string>> LoginAsync(
        string username, string password, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(
                "api/auth/login", new { username, password }, ct);

            if (!response.IsSuccessStatusCode)
                return Result<string>.Failure(
                    Error.Validation("Auth.Invalid", "Credenciais inválidas. Verifique usuário e senha."));

            var payload = await response.Content
                .ReadFromJsonAsync<LoginResponseDto>(cancellationToken: ct);

            return payload?.Token is not null
                ? Result<string>.Success(payload.Token)
                : Result<string>.Failure(
                    Error.Validation("Auth.NoToken", "Token não recebido da API."));
        }
        catch (HttpRequestException ex)
        {
            // Connection refused / DNS failure — expected when the API is not running.
            _logger.LogWarning("API unreachable at login: {Message}", ex.Message);
            return Result<string>.Failure(
                Error.Validation("Auth.Unavailable",
                    "Não foi possível conectar à API. Certifique-se de que o projeto Microondas.Api está em execução."));
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("API login request timed out.");
            return Result<string>.Failure(
                Error.Validation("Auth.Timeout", "A requisição à API excedeu o tempo limite."));
        }
    }

    private sealed record LoginResponseDto(string Token, int ExpiresIn);
}
