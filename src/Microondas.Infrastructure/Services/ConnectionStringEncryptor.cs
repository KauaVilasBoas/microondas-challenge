using System.Security.Cryptography;
using System.Text;

namespace Microondas.Infrastructure.Services;

public static class ConnectionStringEncryptor
{
    private const int KeySize = 256;
    private const int BlockSize = 128;

    public static string Encrypt(string plainText, string key)
    {
        using var aes = CreateAes(key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var combined = new byte[aes.IV.Length + encrypted.Length];
        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
        Buffer.BlockCopy(encrypted, 0, combined, aes.IV.Length, encrypted.Length);

        return Convert.ToBase64String(combined);
    }

    public static string Decrypt(string cipherText, string key)
    {
        var combined = Convert.FromBase64String(cipherText);

        using var aes = CreateAes(key);
        var iv = new byte[aes.BlockSize / 8];
        var encrypted = new byte[combined.Length - iv.Length];

        Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(combined, iv.Length, encrypted, 0, encrypted.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
        return Encoding.UTF8.GetString(decrypted);
    }

    private static Aes CreateAes(string key)
    {
        var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = BlockSize;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = DeriveKey(key, KeySize / 8);
        return aes;
    }

    private static byte[] DeriveKey(string password, int keyBytes)
    {
        using var derive = new Rfc2898DeriveBytes(
            password,
            Encoding.UTF8.GetBytes("MicroondasChallengeSalt"),
            100_000,
            HashAlgorithmName.SHA256);
        return derive.GetBytes(keyBytes);
    }
}
