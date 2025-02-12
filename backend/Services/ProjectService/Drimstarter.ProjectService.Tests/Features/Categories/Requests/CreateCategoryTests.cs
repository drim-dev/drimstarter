using AutoBogus;
using Drimstarter.Common.Database;
using Drimstarter.Common.Utils;
using Drimstarter.ProjectService.Domain;
using Drimstarter.ProjectService.Features.Categories.Requests;
using Drimstarter.ProjectService.Tests.Fixtures;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;

namespace Drimstarter.ProjectService.Tests.Features.Categories.Requests;

[Collection(CategoryTestsCollection.Name)]
public class CreateCategoryTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public CreateCategoryTests(TestFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.Reset(CreateCancellationToken());

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<Client.CreateCategoryReply> Act(Client.CreateCategoryRequest request) =>
        await _fixture.CategoryClient!.CreateCategoryAsync(request, cancellationToken: CreateCancellationToken());

    [Fact]
    public async Task Should_create_category()
    {
        var request = new Client.CreateCategoryRequest { Name = "Art" };

        var reply = await Act(request);

        var categoryDto = reply.Category;

        categoryDto.Should().NotBeNull();
        categoryDto.Id.Should().NotBeEmpty();
        categoryDto.Name.Should().Be(request.Name);

        var dbCategory = await _fixture.Database.SingleOrDefault<Category>(c => c.Id == IdEncoding.Decode(categoryDto.Id),
            CreateCancellationToken());

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task Should_create_category_with_name_in_title_case()
    {
        var request = new Client.CreateCategoryRequest { Name = "art deco" };

        var reply = await Act(request);

        var categoryDto = reply.Category;

        categoryDto.Should().NotBeNull();
        categoryDto.Id.Should().NotBeEmpty();
        categoryDto.Name.Should().Be("Art Deco");

        var dbCategory = await _fixture.Database.SingleOrDefault<Category>(c => c.Id == IdEncoding.Decode(categoryDto.Id),
            CreateCancellationToken());

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be("Art Deco");
    }
}

[Collection(CategoryTestsCollection.Name)]
public class CreateCategoryValidatorTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public CreateCategoryValidatorTests(TestFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.Reset(CreateCancellationToken());

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Should_not_have_errors_when_request_is_valid()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateCategory.RequestValidator>();

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
        var validator = scope.ServiceProvider.GetRequiredService<CreateCategory.RequestValidator>();

        var request = CreateRequest(name);

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode("categories:validation:name_empty");
    }

    [Fact]
    public async Task Should_have_error_when_name_less_min_length()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateCategory.RequestValidator>();

        var request = CreateRequest("a");

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode("categories:validation:name_less_min_length");
    }

    [Fact]
    public async Task Should_have_error_when_name_greater_max_length()
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateCategory.RequestValidator>();

        var request = CreateRequest(new string('a', 51));

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode("categories:validation:name_greater_max_length");
    }

    [Theory]
    [InlineData("Art")]
    [InlineData("art")]
    public async Task Should_have_error_when_name_already_exists(string name)
    {
        await using var scope = _fixture.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<CreateCategory.RequestValidator>();

        var existingCategory = new Category { Name = name.CapitalizeWords() };
        await _fixture.Database.Save(existingCategory);

        var request = CreateRequest(name);

        var result = await validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode("categories:validation:name_already_exists");
    }

    private static CreateCategoryRequest CreateRequest(string? name = null) =>
        new AutoFaker<CreateCategoryRequest>()
            .RuleFor(r => r.Name, f => name ?? f.Random.Word())
            .Generate();
}
