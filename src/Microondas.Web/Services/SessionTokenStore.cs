namespace Microondas.Web.Services;

public sealed class SessionTokenStore : ITokenStore
{
    private const string TokenKey = "ApiToken";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionTokenStore(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public string? GetToken()
        => _httpContextAccessor.HttpContext?.Session.GetString(TokenKey);

    public void SetToken(string token)
        => _httpContextAccessor.HttpContext?.Session.SetString(TokenKey, token);

    public void ClearToken()
        => _httpContextAccessor.HttpContext?.Session.Remove(TokenKey);

    public bool IsAuthenticated()
        => GetToken() is not null;
}
