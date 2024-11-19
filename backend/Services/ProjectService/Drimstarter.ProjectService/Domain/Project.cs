namespace Drimstarter.ProjectService.Domain;

public class Project
{
    public const int TitleMaxLength = 100;
    public const int DescriptionMaxLength = 500;
    public const int StoryMaxLength = 5000;

    public long Id { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required string Story { get; init; }

    public required decimal FundingGoal { get; init; }

    public required decimal CurrentFunding { get; init; }

    public required DateTime StartDate { get; init; }

    public required DateTime EndDate { get; init; }

    public required ProjectStatus Status { get; init; }

    public Category? Category { get; init; }

    public required long CategoryId { get; init; }

    public required long UserId { get; init; }
}

public enum ProjectStatus
{
    Draft,
    OnReview,
    Active,
    Completed,
}
