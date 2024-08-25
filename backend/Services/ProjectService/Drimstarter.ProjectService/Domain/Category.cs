namespace Drimstarter.ProjectService.Domain;

public class Category
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 50;

    public long Id { get; set; }

    public required string Name { get; init; }
}
