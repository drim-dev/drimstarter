namespace Drimstarter.ProjectService.Features.Categories.Errors;

public static class CategoryValidationErrors
{
    private const string Prefix = "categories:validation:";

    public const string NameEmpty = Prefix + "name_empty";
    public const string NameLessMinLength = Prefix + "name_less_min_length";
    public const string NameGreaterMaxLength = Prefix + "name_greater_max_length";
    public const string NameAlreadyExists = Prefix + "name_already_exists";
}
