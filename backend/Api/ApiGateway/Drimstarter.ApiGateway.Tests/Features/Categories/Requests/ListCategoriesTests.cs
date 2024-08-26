using System.Net.Http.Json;
using Drimstarter.ApiGateway.Tests.Features.Categories.Contracts;
using Drimstarter.ApiGateway.Tests.Fixtures;
using Drimstarter.ApiGateway.Tests.Utils;
using Drimstarter.ProjectService;
using FluentAssertions;

namespace Drimstarter.ApiGateway.Tests.Features.Categories.Requests;

[Collection(TestsCollection.Name)]
public class ListCategoriesTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public ListCategoriesTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => _fixture.Reset();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<CategoryContract[]?> Act()
    {
        var response = await _fixture.CreateClient().GetAsync("/categories", CreateCancellationToken());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryContract[]>();
    }

    [Fact]
    public async Task Should_return_categories()
    {
        var categoryDtos = FakeFactory.CreateCategoryDtos();

        _fixture.ProjectService.MockResponse<ListCategoriesRequest, ListCategoriesReply>(_ => new()
        {
            Categories = { categoryDtos },
        });

        var replyCategories = await Act();

        replyCategories.Should().NotBeNull();
        replyCategories!.Length.Should().Be(categoryDtos.Count);

        for (var i = 0; i < categoryDtos.Count; i++)
        {
            replyCategories[i].Id.Should().Be(categoryDtos[i].Id);
            replyCategories[i].Name.Should().Be(categoryDtos[i].Name);
        }
    }
}
