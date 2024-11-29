using Drimstarter.Common.Database;
using Drimstarter.Common.Tests.Assertions;
using Drimstarter.ProjectService.Tests.Fixtures;
using Drimstarter.ProjectService.Tests.Utils;
using FluentAssertions;

namespace Drimstarter.ProjectService.Tests.Features.Categories.Requests;

[Collection(CategoryTestsCollection.Name)]
public class ListCategoriesTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public ListCategoriesTests(TestFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.Reset(CreateCancellationToken());

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<Client.ListCategoriesReply> Act()
    {
        var request = new Client.ListCategoriesRequest();
        return await _fixture.CategoryClient!.ListCategoriesAsync(request,
            cancellationToken: CreateCancellationToken());
    }

    [Fact]
    public async Task Should_list_categories()
    {
        var dbCategories = FakeFactory.CreateCategories();

        await _fixture.Database.Save(dbCategories);

        var reply = await Act();

        reply.Categories.Should().NotBeEmpty();

        reply.Categories.ShouldBeEquivalentTo(dbCategories,
            (a, e) => a.Id == IdEncoding.Encode(e.Id),
            (a, e) => a.Name.Should().Be(e.Name));
    }
}
