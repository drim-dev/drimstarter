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
}
