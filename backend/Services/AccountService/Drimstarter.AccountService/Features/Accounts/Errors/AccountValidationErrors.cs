namespace Drimstarter.AccountService.Features.Accounts.Errors;

public static class AccountValidationErrors
{
    private const string Prefix = "accounts:validation:";

    public const string NameEmpty = Prefix + "name_empty";
    public const string NameLessMinLength = Prefix + "name_less_min_length";
    public const string NameGreaterMaxLength = Prefix + "name_greater_max_length";

    public const string EmailInvalidFormat = Prefix + "email_invalid_format";
    public const string EmailAlreadyExists = Prefix + "email_already_exists";

    public const string PasswordEmpty = Prefix + "password_empty";
    public const string PasswordLessMinLength = Prefix + "password_less_min_length";
    public const string PasswordGreaterMaxLength = Prefix + "password_greater_max_length";
    public const string PasswordNoUppercase = Prefix + "password_no_uppercase";
    public const string PasswordNoLowercase = Prefix + "password_no_lowercase";
    public const string PasswordNoNumber = Prefix + "password_no_number";

}
