namespace Drimstarter.ProjectService.Features.Projects.Errors;

public static class ProjectValidationErrors
{
    private const string Prefix = "projects:validation:";

    public const string CategoryIdInvalid = Prefix + "category_id_invalid";
    public const string MaxPageSizeInvalid = Prefix + "max_page_size_invalid";
    public const string SortInvalid = Prefix + "sort_invalid";
    public const string PageTokenInvalid = Prefix + "page_token_invalid";
}
