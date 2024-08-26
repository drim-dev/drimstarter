using Drimstarter.Common.Database;
using Drimstarter.ProjectService.Tests.Fixtures;
using Drimstarter.ProjectService.Tests.Utils;
using FluentAssertions;
using IdGen;

namespace Drimstarter.ProjectService.Tests.Features.Categories.Requests;

[Collection(CategoryTestsCollection.Name)]
public class ListCategoriesTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public ListCategoriesTests(TestFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.Reset(CreateCancellationToken());

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<Client.ListCategoriesReply> Act(Client.ListCategoriesRequest request) =>
        await _fixture.CategoryClient!.ListCategoriesAsync(request, cancellationToken: CreateCancellationToken());

    [Fact]
    public async Task Should_list_categories()
    {
        var dbCategories = FakeFactory.CreateCategories();

        await _fixture.Database.Save(dbCategories);

        var request = new Client.ListCategoriesRequest();

        var reply = await Act(request);

        reply.Categories.Should().NotBeEmpty();

        // TODO: move to extension
        reply.Categories.Should().HaveCount(dbCategories.Count);
        foreach (var dbCategory in dbCategories)
        {
            reply.Categories.Should().Contain(c =>
                c.Id == IdEncoding.Encode(dbCategory.Id) &&
                c.Name == dbCategory.Name);
        }
    }
}
