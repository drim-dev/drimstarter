using Drimstarter.ProjectService.Tests.Fixtures;

namespace Drimstarter.ProjectService.Tests.Features.Projects;

[CollectionDefinition(Name)]
public class ProjectTestsCollection : ICollectionFixture<TestFixture>
{
    public const string Name = nameof(ProjectTestsCollection);
}
