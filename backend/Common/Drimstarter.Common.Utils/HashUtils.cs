using System.Security.Cryptography;
using System.Text;

namespace Drimstarter.Common.Utils;

public static class HashUtils
{
    public static int HashSha256(string text)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return BitConverter.ToInt32(hash, 0);
    }
}
