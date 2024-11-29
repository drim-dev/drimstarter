namespace Drimstarter.ApiGateway.Features.Projects.Models;

public record ProjectModel(string Id, string Title, string Description, string Story, decimal FundingGoal,
    decimal CurrentFunding, DateTime StartDate, DateTime EndDate);
