using System.Text;
using Drimstarter.AccountService.Features.Accounts.Options;
using Konscious.Security.Cryptography;

namespace Drimstarter.AccountService.Features.Accounts.Services;

// TODO: test
// TODO: refactor
public static class PasswordHasher
{
    public static string HashPassword(string password, PasswordHashingOptions options)
    {
        var salt = new byte[options.SaltSize];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = options.Parallelism,
            MemorySize = options.MemorySize,
            Iterations = options.Iterations,
        };

        var hashBytes = argon2.GetBytes(options.HashSize);
        var hashString = Convert.ToBase64String(hashBytes);

        var saltString = Convert.ToBase64String(salt);

        return $"argon2id$v=19$m={options.MemorySize},t={options.Iterations},p={options.Parallelism}${saltString}${hashString}";
    }

    public static bool VerifyPassword(string password, string hash)
    {
        var parts = hash.Split('$');
        if (parts.Length != 5)
        {
            return false;
        }

        if (parts[0] != "argon2id")
        {
            return false;
        }

        if (parts[1] != "v=19")
        {
            return false;
        }

        int? memorySize = null;
        int? iterations = null;
        int? parallelism = null;

        foreach (var part in parts[2].Split(','))
        {
            var kv = part.Split('=');
            if (kv.Length != 2)
            {
                return false;
            }

            switch (kv[0])
            {
                case "m" when int.TryParse(kv[1], out var m):
                    memorySize = m;
                    break;
                case "t" when int.TryParse(kv[1], out var t):
                    iterations = t;
                    break;
                case "p" when int.TryParse(kv[1], out var p):
                    parallelism = p;
                    break;
                default:
                    return false;
            }
        }

        if (memorySize is null || iterations is null || parallelism is null)
        {
            return false;
        }

        var saltBytes = Convert.FromBase64String(parts[3]);
        var hashBytes = Convert.FromBase64String(parts[4]);

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = saltBytes,
            DegreeOfParallelism = parallelism.Value,
            MemorySize = memorySize.Value,
            Iterations = iterations.Value,
        };

        var hashBytes2 = argon2.GetBytes(hashBytes.Length);

        return hashBytes.SequenceEqual(hashBytes2);
    }
}
