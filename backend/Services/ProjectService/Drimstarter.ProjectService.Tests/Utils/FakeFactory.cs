using AutoBogus;
using Drimstarter.ProjectService.Domain;
using IdGen;

namespace Drimstarter.ProjectService.Tests.Utils;

public static class FakeFactory
{
    private static readonly IdGenerator IdGenerator = new(1);

    public static T Create<T>() => AutoFaker.Generate<T>();

    public static List<Category> CreateCategories(int count = 5) =>
        new AutoFaker<Category>()
            .RuleFor(x => x.Id, _ => IdGenerator.CreateId())
            .Generate(count);

    public static Category CreateCategory() => CreateCategories(1).Single();

    public static List<Project> CreateProjects(long categoryId, int count = 5) =>
        new AutoFaker<Project>()
            .RuleFor(x => x.Id, _ => IdGenerator.CreateId())
            .RuleFor(x => x.FundingGoal, f => f.Finance.Amount())
            .RuleFor(x => x.CurrentFunding, f => f.Finance.Amount())
            .RuleFor(x => x.StartDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(x => x.EndDate, f => f.Date.Future().ToUniversalTime())
            .RuleFor(x => x.CategoryId, f => categoryId)
            .RuleFor(x => x.Category, _ => null)
            .Generate(count);

    public static Project CreateProject(long categoryId) => CreateProjects(categoryId, 1).Single();
}
