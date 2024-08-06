using AutoBogus;
using Drimstarter.ProjectService.Domain;
using Drimstarter.ProjectService.Tests.Integration.Fixtures;
using Drimstarter.ProjectService.Tests.Utils;
using FluentAssertions;

namespace Drimstarter.ProjectService.Tests.Integration.Features.Categories.Requests;

[Collection(CategoryTestsCollection.Name)]
public class ListCategoriesTests(TestFixture _fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => _fixture.Reset(CreateCancellationToken());

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<Client.ListCategoriesReply> Act(Client.ListCategoriesRequest request) =>
        await _fixture.CategoryClient!.ListCategoriesAsync(request, cancellationToken: CreateCancellationToken());

    [Fact]
    public async Task Should_list_categories()
    {
        var categories = FakeFactory.CreateCategories();

        await _fixture.Database.Save(categories);

        var request = new Client.ListCategoriesRequest();

        var reply = await Act(request);

        reply.Categories.Should().NotBeEmpty();

        reply.Categories.Should().BeEquivalentTo(categories, options => options.ExcludingMissingMembers());
    }
}
