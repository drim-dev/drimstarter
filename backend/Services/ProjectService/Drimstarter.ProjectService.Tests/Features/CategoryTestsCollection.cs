using Drimstarter.ProjectService.Tests.Fixtures;

namespace Drimstarter.ProjectService.Tests.Features;

[CollectionDefinition(Name)]
public class CategoryTestsCollection : ICollectionFixture<TestFixture>
{
    public const string Name = nameof(CategoryTestsCollection);
}
