using System.Net.Http.Json;
using Drimstarter.ApiGateway.Tests.Features.Categories.Contracts;
using Drimstarter.ApiGateway.Tests.Fixtures;
using Drimstarter.ApiGateway.Tests.Utils;
using Drimstarter.ProjectService;
using FluentAssertions;

namespace Drimstarter.ApiGateway.Tests.Features.Categories.Requests;

[Collection(TestsCollection.Name)]
public class CreateCategoryTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public CreateCategoryTests(TestFixture fixture) => _fixture = fixture;

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
    public async Task Should_create_category_using_project_service()
    {
        var categoryDto = FakeFactory.CreateCategoryDto();

        var mock = _fixture.ProjectService.MockResponse<CreateCategoryRequest, CreateCategoryReply>(_ => new()
        {
            Category = categoryDto,
        });

        var replyCategory = await Act(categoryDto.Name);

        mock.Request.Name.Should().Be(categoryDto.Name);

        replyCategory.Should().NotBeNull();
        replyCategory!.Id.Should().Be(categoryDto.Id);
        replyCategory.Name.Should().Be(categoryDto.Name);
    }
}
