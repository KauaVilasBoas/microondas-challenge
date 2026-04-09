using Microondas.SharedKernel;

namespace Microondas.Web.Services;

/// <summary>
/// Authenticates the user against the REST API and returns a Bearer token.
/// This is the only operation that crosses the process boundary in the Web project —
/// all business operations remain local and are dispatched via MediatR.
/// </summary>
public interface IApiAuthService
{
    Task<Result<string>> LoginAsync(string username, string password, CancellationToken ct = default);
}