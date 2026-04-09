namespace Microondas.Web.Services;

public interface ITokenStore
{
    string? GetToken();
    void SetToken(string token);
    void ClearToken();
    bool IsAuthenticated();
}
