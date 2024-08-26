using Drimstarter.ApiGateway.Tests.Fixtures;

namespace Drimstarter.ApiGateway.Tests.Features;

[CollectionDefinition(Name)]
public class TestsCollection : ICollectionFixture<TestFixture>
{
    public const string Name = nameof(TestsCollection);
}
