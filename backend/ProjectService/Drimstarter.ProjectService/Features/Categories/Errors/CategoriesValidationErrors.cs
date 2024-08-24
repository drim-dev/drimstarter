namespace Drimstarter.ProjectService.Features.Categories.Errors;

public static class CategoriesValidationErrors
{
    private const string Prefix = "categories:validation:";

    public const string NameMustNotBeEmpty = Prefix + "name_must_not_be_empty";
    public const string NameMustBeGreaterOrEqualMinLength = Prefix + "name_must_be_greater_or_equal_min_length";
    public const string NameMustBeLessOrEqualMaxLength = Prefix + "name_must_be_less_or_equal_max_length";
    public const string NameMustNotAlreadyExist = Prefix + "name_must_not_already_exist";
}
