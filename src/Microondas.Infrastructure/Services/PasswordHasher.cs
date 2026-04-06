using System.Security.Cryptography;
using System.Text;

namespace Microondas.Infrastructure.Services;

public static class PasswordHasher
{
    public static string HashSha256(string plainText)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainText));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static bool Verify(string plainText, string hashedValue) =>
        HashSha256(plainText) == hashedValue;
}
