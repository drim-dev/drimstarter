using Drimstarter.ApiGateway.Tests.Integration.Fixtures;

namespace Drimstarter.ApiGateway.Tests.Integration.Features;

[CollectionDefinition(Name)]
public class CategoryTestsCollection : ICollectionFixture<TestFixture>
{
    public const string Name = nameof(CategoryTestsCollection);
}
