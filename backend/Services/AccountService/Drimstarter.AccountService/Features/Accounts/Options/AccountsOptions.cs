using Konscious.Security.Cryptography;

namespace Drimstarter.AccountService.Features.Accounts.Options;

public class AccountsOptions
{
    public PasswordHashingOptions PasswordHashing { get; set; } = new();
}

public class PasswordHashingOptions
{
    public int Iterations { get; set; } = 4;
    public int MemorySize { get; set; } = 1_024 * 128;
    public int Parallelism { get; set; } = 8;
    public int SaltSize { get; set; } = 16;
    public int HashSize { get; set; } = 32;
}
