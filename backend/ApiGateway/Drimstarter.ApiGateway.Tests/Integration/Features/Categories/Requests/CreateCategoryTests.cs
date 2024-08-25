using System.Net.Http.Json;
using Drimstarter.ApiGateway.Tests.Integration.Features.Categories.Contracts;
using Drimstarter.ApiGateway.Tests.Integration.Fixtures;
using Drimstarter.ApiGateway.Tests.Utils;
using Drimstarter.ProjectService;
using FluentAssertions;

namespace Drimstarter.ApiGateway.Tests.Integration.Features.Categories.Requests;

[Collection(CategoryTestsCollection.Name)]
public class CreateCategoryTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public CreateCategoryTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => _fixture.Reset();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<CategoryContract?> Act(string name)
    {
        var response = await _fixture.CreateClient().PostAsJsonAsync("/categories", new { name },
            CreateCancellationToken());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryContract>();
    }

    [Fact]
    public async Task Should_create_category()
    {
        var categoryDto = FakeFactory.CreateCategoryDto();

        var mock = _fixture.ProjectService.MockResponse<CreateCategoryRequest, CreateCategoryReply>(_ => new()
        {
            Category = categoryDto,
        });

        var category = await Act(categoryDto.Name);

        mock.Request.Name.Should().Be(categoryDto.Name);

        category.Should().NotBeNull();
        category!.Id.Should().Be(categoryDto.Id);
        category.Name.Should().Be(categoryDto.Name);
    }
}
