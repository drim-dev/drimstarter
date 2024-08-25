extern alias api;

using Drimstarter.Common.Tests.Harnesses;
using Drimstarter.ProjectService.Harnesses;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Drimstarter.ApiGateway.Tests.Integration.Fixtures;

public class TestFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<api::Program> _factory;

    public TestFixture()
    {
        ProjectService = new();

        _factory = new WebApplicationFactory<api::Program>()
            .AddHarness(ProjectService);
    }

    public ProjectServiceHarness<api::Program> ProjectService { get; }

    public HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    public Task Reset()
    {
        ProjectService.Reset();

        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await ProjectService.Start(_factory, CreateCancellationToken(60));

        _ = _factory.Server;
    }

    public async Task DisposeAsync()
    {
        await ProjectService.Stop(CreateCancellationToken());
    }
}
