using Drimstarter.Common.Grpc.Client;
using Drimstarter.Common.Tests.Database.Harnesses;
using Drimstarter.Common.Tests.Grpc.Harnesses.Client;
using Drimstarter.Common.Tests.Harnesses;
using Drimstarter.ProjectService.Database;
using Drimstarter.ServiceDefaults;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Drimstarter.ProjectService.Tests.Fixtures;

public class TestFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly GrpcChannelHarness<Program> _grpcChannel;

    public TestFixture()
    {
        Database = new(ResourceNames.ProjectServiceDb);
        _grpcChannel = new();

        _factory = new WebApplicationFactory<Program>()
            .AddHarness(Database)
            .AddHarness(_grpcChannel);
    }

    public DatabaseHarness<Program, ProjectDbContext> Database { get; }
    public Client.Categories.CategoriesClient? CategoryClient { get; private set; }
    public Client.Projects.ProjectsClient? ProjectClient { get; private set; }

    public AsyncServiceScope CreateScope() => _factory.Services.CreateAsyncScope();

    public async Task Reset(CancellationToken cancellationToken)
    {
        await Database.Clear(cancellationToken);
    }

    public async Task InitializeAsync()
    {
        await Database.Start(_factory, CreateCancellationToken(60));

        _ = _factory.Server;

        await _grpcChannel.Start(_factory, CreateCancellationToken(60));

        CategoryClient = new Client.Categories.CategoriesClient(_grpcChannel.GrpcChannel);
        ProjectClient = new Client.Projects.ProjectsClient(_grpcChannel.GrpcChannel);

        await Database.Migrate(CreateCancellationToken());
    }

    public async Task DisposeAsync()
    {
        await _grpcChannel.Stop(CreateCancellationToken());
        await Database.Stop(CreateCancellationToken());
    }
}
