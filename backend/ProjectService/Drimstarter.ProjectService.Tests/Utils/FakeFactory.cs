using AutoBogus;
using Drimstarter.ProjectService.Domain;

namespace Drimstarter.ProjectService.Tests.Utils;

public static class FakeFactory
{
    public static T Create<T>() => AutoFaker.Generate<T>();

    public static List<Category> CreateCategories(int count = 5) =>
        new AutoFaker<Category>()
            .Ignore(x => x.Id)
            .Generate(count);
}
