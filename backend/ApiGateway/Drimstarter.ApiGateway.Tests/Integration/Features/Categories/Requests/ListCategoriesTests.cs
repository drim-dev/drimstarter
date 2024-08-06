using System.Net.Http.Json;
using Drimstarter.ApiGateway.Tests.Integration.Features.Categories.Contracts;
using Drimstarter.ApiGateway.Tests.Integration.Fixtures;
using Drimstarter.ApiGateway.Tests.Utils;
using Drimstarter.ProjectService;
using FluentAssertions;

namespace Drimstarter.ApiGateway.Tests.Integration.Features.Categories.Requests;

[Collection(CategoryTestsCollection.Name)]
public class ListCategoriesTests(TestFixture _fixture) : IAsyncLifetime
{
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

        var categories = await Act();

        categories.Should().NotBeNull();
        categories!.Length.Should().Be(categoryDtos.Count);

        for (var i = 0; i < categoryDtos.Count; i++)
        {
            categories[i].Id.Should().Be((short)categoryDtos[i].Id);
            categories[i].Name.Should().Be(categoryDtos[i].Name);
        }
    }
}
