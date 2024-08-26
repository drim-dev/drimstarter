extern alias api;
using Drimstarter.AccountService.Harnesses;
using Drimstarter.Common.Tests.Harnesses;
using Drimstarter.ProjectService.Harnesses;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Drimstarter.ApiGateway.Tests.Fixtures;

public class TestFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<api::Program> _factory;

    public TestFixture()
    {
        ProjectService = new();
        AccountService = new();

        _factory = new WebApplicationFactory<api::Program>()
            .AddHarness(ProjectService)
            .AddHarness(AccountService);
    }

    public ProjectServiceHarness<api::Program> ProjectService { get; }
    public AccountServiceHarness<api::Program> AccountService { get; }

    public HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    public Task Reset()
    {
        ProjectService.Reset();
        AccountService.Reset();

        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await ProjectService.Start(_factory, CreateCancellationToken(60));
        await AccountService.Start(_factory, CreateCancellationToken(60));

        _ = _factory.Server;
    }

    public async Task DisposeAsync()
    {
        await AccountService.Stop(CreateCancellationToken());
        await ProjectService.Stop(CreateCancellationToken());
    }
}
