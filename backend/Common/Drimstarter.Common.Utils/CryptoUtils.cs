using System.Security.Cryptography;
using System.Text;

namespace Drimstarter.Common.Utils;

public static class CryptoUtils
{
    public static byte[] EncryptAes(string plainText, byte[] encryptionKey, byte[] iv)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        using var aes = Aes.Create();
        var encryptor = aes.CreateEncryptor(encryptionKey, iv);

        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();

        return memoryStream.ToArray();
    }

    public static bool TryDecryptAes(byte[] cypherTextBytes, byte[] encryptionKey, byte[] iv, out string? plainText)
    {
        try
        {
            using var aes = Aes.Create();
            var decryptor = aes.CreateDecryptor(encryptionKey, iv);

            using var memoryStream = new MemoryStream(cypherTextBytes);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            plainText = streamReader.ReadToEnd();

            return true;
        }
        catch
        {
            plainText = null;
            return false;
        }
    }
}
