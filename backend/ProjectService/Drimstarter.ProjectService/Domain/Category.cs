namespace Drimstarter.ProjectService.Domain;

public class Category
{
    public const int NameMaxLength = 50;

    public short Id { get; set; }

    public required string Name { get; init; }
}
