namespace Drimstarter.AccountService.Domain;

public class Account
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 100;

    public const int EmailMaxLength = 100;

    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 100;

    public long Id { get; set; }

    public required string Name { get; init; }

    public required string Email { get; init; }

    public required string PasswordHash { get; init; }
}
