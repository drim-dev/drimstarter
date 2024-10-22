using Drimstarter.AccountService.Database;
using Drimstarter.Common.Tests.Database.Harnesses;
using Drimstarter.Common.Tests.Grpc.Harnesses.Client;
using Drimstarter.Common.Tests.Harnesses;
using Drimstarter.ServiceDefaults;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Drimstarter.AccountService.Tests.Fixtures;

public class TestFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly GrpcChannelHarness<Program> _grpcChannel;

    public TestFixture()
    {
        Database = new(ResourceNames.AccountServiceDb);
        _grpcChannel = new();

        _factory = new WebApplicationFactory<Program>()
            .AddHarness(Database)
            .AddHarness(_grpcChannel);
    }

    public DatabaseHarness<Program, AccountDbContext> Database { get; }
    public Client.Accounts.AccountsClient? AccountClient { get; private set; }

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

        AccountClient = new Client.Accounts.AccountsClient(_grpcChannel.GrpcChannel);

        await Database.Migrate(CreateCancellationToken());
    }

    public async Task DisposeAsync()
    {
        await _grpcChannel.Stop(CreateCancellationToken());
        await Database.Stop(CreateCancellationToken());
    }
}
