extern alias api;
using System.Net.Http.Json;
using Drimstarter.AccountService;
using Drimstarter.ApiGateway.Tests.Features.Accounts.Contracts;
using Drimstarter.ApiGateway.Tests.Fixtures;
using Drimstarter.ApiGateway.Tests.Utils;
using FluentAssertions;

namespace Drimstarter.ApiGateway.Tests.Features.Accounts.Requests;

[Collection(TestsCollection.Name)]
public class CreateAccountTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public CreateAccountTests(TestFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.Reset();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<AccountContract?> Act(string name, string email, string password)
    {
        var response = await _fixture.CreateClient().PostAsJsonAsync("/accounts", new { name, email, password },
            CreateCancellationToken());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AccountContract>();
    }

    [Fact]
    public async Task Should_create_account_using_account_service()
    {
        var accountDto = FakeFactory.CreateAccountDto();

        var mock = _fixture.AccountService.MockResponse<CreateAccountRequest, CreateAccountReply>(_ => new()
        {
            Account = accountDto,
        });

        const string password = "password";
        var replyAccount = await Act(accountDto.Name, accountDto.Email, password);

        mock.Request.Should().NotBeNull();
        mock.Request!.Name.Should().Be(accountDto.Name);
        mock.Request.Email.Should().Be(accountDto.Email);
        mock.Request.Password.Should().Be(password);

        replyAccount.Should().NotBeNull();
        replyAccount!.Id.Should().Be(accountDto.Id);
        replyAccount.Name.Should().Be(accountDto.Name);
        replyAccount.Email.Should().Be(accountDto.Email);
    }
}
