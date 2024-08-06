using Drimstarter.ProjectService.Tests.Integration.Fixtures;

namespace Drimstarter.ProjectService.Tests.Integration.Features;

[CollectionDefinition(Name)]
public class CategoryTestsCollection : ICollectionFixture<TestFixture>
{
    public const string Name = nameof(CategoryTestsCollection);
}
