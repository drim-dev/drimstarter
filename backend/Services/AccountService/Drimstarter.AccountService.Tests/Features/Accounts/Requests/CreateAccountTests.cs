using AutoBogus;
using Drimstarter.AccountService.Domain;
using Drimstarter.AccountService.Features.Accounts.Requests;
using Drimstarter.AccountService.Features.Accounts.Services;
using Drimstarter.AccountService.Tests.Fixtures;
using Drimstarter.Common.Database;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;

namespace Drimstarter.AccountService.Tests.Features.Accounts.Requests;

[Collection(AccountTestsCollection.Name)]
public class CreateAccountTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public CreateAccountTests(TestFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.Reset(CreateCancellationToken());

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<Client.CreateAccountReply> Act(Client.CreateAccountRequest request) =>
        await _fixture.AccountClient!.CreateAccountAsync(request, cancellationToken: CreateCancellationToken());

    [Fact]
    public async Task Should_create_account()
    {
        var request = new Client.CreateAccountRequest
        {
            Name = "John Doe",
            Email = "john@doe.com",
            Password = "Password1",
        };

        var reply = await Act(request);

        var accountDto = reply.Account;

        accountDto.Should().NotBeNull();
        accountDto.Id.Should().NotBeEmpty();
        accountDto.Name.Should().Be(request.Name);
        accountDto.Email.Should().Be(request.Email);

        var account = await _fixture.Database.SingleOrDefault<Account>(c => c.Id == IdEncoding.Decode(accountDto.Id),
            CreateCancellationToken());

        account.Should().NotBeNull();
        account!.Name.Should().Be(request.Name);
        account.Email.Should().Be(request.Email);

        PasswordHasher.VerifyPassword(request.Password, account.PasswordHash).Should().BeTrue();
    }
}

// TODO: refactor to extract all logic up to request validation into a helper method
[Collection(AccountTestsCollection.Name)]
public class CreateAccountValidatorTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public CreateAccountValidatorTests(TestFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.Reset(CreateCancellationToken());

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Should_not_have_errors_when_request_is_valid()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest();

        var result = await validator.TestValidateAsync(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    // TODO: [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Should_have_error_when_name_empty(string name)
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest(name);

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode("accounts:validation:name_empty");
    }

    [Fact]
    public async Task Should_have_error_when_name_less_min_length()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest("a");

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode("accounts:validation:name_less_min_length");
    }

    [Fact]
    public async Task Should_have_error_when_name_greater_max_length()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest(new string('a', 101));

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode("accounts:validation:name_greater_max_length");
    }

    [Theory]
    // TODO: [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("test")]
    [InlineData("test@")]
    [InlineData("@test")]
    public async Task Should_have_error_when_email_invalid_format(string email)
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest(email: email);

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorCode("accounts:validation:email_invalid_format");
    }

    [Theory]
    [InlineData("test@test.com")]
    [InlineData("Test@Test.Com")]
    public async Task Should_have_error_when_email_already_exists_ignoring_case(string email)
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var existingAccount = new Account
        {
            Id = 1,
            Name = "Test User",
            Email = email.ToLower(),
            PasswordHash = "password",
        };
        await _fixture.Database.Save(existingAccount);

        var request = CreateRequest(email: email);

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorCode("accounts:validation:email_already_exists");
    }

    [Theory]
    // TODO: [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Should_have_error_when_password_empty(string password)
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest(password: password);

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode("accounts:validation:password_empty");
    }

    [Fact]
    public async Task Should_have_error_when_password_less_min_length()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest(password: "Passwd1");

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode("accounts:validation:password_less_min_length");
    }

    [Fact]
    public async Task Should_have_error_when_password_greater_max_length()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest(password: new string('a', 99) + "A1");

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode("accounts:validation:password_greater_max_length");
    }

    [Fact]
    public async Task Should_have_error_when_password_no_uppercase()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest(password: "password1");

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode("accounts:validation:password_no_uppercase");
    }

    [Fact]
    public async Task Should_have_error_when_password_no_lowercase()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest(password: "PASSWORD1");

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode("accounts:validation:password_no_lowercase");
    }

    [Fact]
    public async Task Should_have_error_when_password_no_number()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateAccount.RequestValidator>();

        var request = CreateRequest(password: "Password");

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode("accounts:validation:password_no_number");
    }

    private static CreateAccountRequest CreateRequest(string? name = null, string? email = null,string? password = null) =>
        new AutoFaker<CreateAccountRequest>()
            .RuleFor(r => r.Name, f => name ?? f.Random.Word())
            .RuleFor(r => r.Email, f => email ?? f.Internet.Email())
            .RuleFor(r => r.Password, f => password ?? "Password1")
            .Generate();
}
