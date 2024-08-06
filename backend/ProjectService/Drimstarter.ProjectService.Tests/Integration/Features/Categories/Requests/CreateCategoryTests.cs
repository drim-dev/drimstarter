using Drimstarter.ProjectService.Domain;
using Drimstarter.ProjectService.Tests.Integration.Fixtures;
using FluentAssertions;

namespace Drimstarter.ProjectService.Tests.Integration.Features.Categories.Requests;

[Collection(CategoryTestsCollection.Name)]
public class CreateCategoryTests(TestFixture _fixture) : IAsyncLifetime
{
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
        categoryDto.Id.Should().BeGreaterOrEqualTo(0);
        categoryDto.Name.Should().Be(request.Name);

        var category = await _fixture.Database.SingleOrDefault<Category>(c => c.Id == categoryDto.Id,
            CreateCancellationToken());

        category.Should().NotBeNull();
        category!.Name.Should().Be(request.Name);
    }

    // TODO: test should capitalize
}
