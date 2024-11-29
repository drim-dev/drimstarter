using Drimstarter.Common.Database;
using Drimstarter.Common.Grpc.Shared.Utils;
using Drimstarter.Common.Tests.Assertions;
using Drimstarter.Common.Tests.Grpc.Assertions;
using Drimstarter.ProjectService.Domain;
using Drimstarter.ProjectService.Tests.Features.Categories;
using Drimstarter.ProjectService.Tests.Fixtures;
using Drimstarter.ProjectService.Tests.Utils;
using FluentAssertions;

namespace Drimstarter.ProjectService.Tests.Features.Projects.Requests;

[Collection(ProjectTestsCollection.Name)]
public class ListProjectsTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;
    private Category? _dbCategory;

    public ListProjectsTests(TestFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync()
    {
        await _fixture.Reset(CreateCancellationToken());

        _dbCategory = FakeFactory.CreateCategory();
        await _fixture.Database.Save(_dbCategory);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<Client.ListProjectsReply> Act(string? categoryId = null, string? sort = null,
        string? pageToken = null, int? maxPageSize = null)
    {
        var request = new Client.ListProjectsRequest
        {
            CategoryId = categoryId,
            Sort = sort,
            PageToken = pageToken,
            MaxPageSize = maxPageSize,
        };
        return await _fixture.ProjectClient!.ListProjectsAsync(request, cancellationToken: CreateCancellationToken());
    }

    [Fact]
    public async Task Should_list_projects()
    {
        var dbProjects = FakeFactory.CreateProjects(_dbCategory!.Id);
        await _fixture.Database.Save(dbProjects);

        var reply = await Act();

        reply.Projects.Should().NotBeEmpty();

        reply.Projects.ShouldBeEquivalentTo(dbProjects,
            (a, e) => a.Id == IdEncoding.Encode(e.Id),
            (a, e) => a.ShouldBeEquivalentTo(e));
    }

    [Fact]
    public async Task Should_filter_by_categoryId()
    {
        var category1 = FakeFactory.CreateCategory();
        var category2 = FakeFactory.CreateCategory();
        await _fixture.Database.Save(category1, category2);

        var project1 = FakeFactory.CreateProject(category1.Id);
        var project2 = FakeFactory.CreateProject(category2.Id);
        await _fixture.Database.Save(project1, project2);

        var reply = await Act(categoryId: IdEncoding.Encode(category1.Id));

        reply.Projects.Should().HaveCount(1);
        reply.Projects.Single().Id.Should().Be(IdEncoding.Encode(project1.Id));
    }

    [Fact]
    public async Task Should_sort_by_StartDate_descending_by_default()
    {
        var projects = FakeFactory.CreateProjects(_dbCategory!.Id);
        await _fixture.Database.Save(projects);

        var reply = await Act();

        reply.Projects.Select(x => x.StartDate).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Should_sort()
    {
        var projects = FakeFactory.CreateProjects(_dbCategory!.Id, 10);
        await _fixture.Database.Save(projects);

        await TestSort("-startdate", x => x.StartDate.ToDateTime(), false);
        await TestSort("startdate", x => x.StartDate.ToDateTime(), true);
        await TestSort("-enddate", x => x.EndDate.ToDateTime(), false);
        await TestSort("enddate", x => x.EndDate.ToDateTime(), true);
        await TestSort("-title", x => x.Title, false);
        await TestSort("title", x => x.Title, true);
        await TestSort("-fundinggoal", x => x.FundingGoal.FromGrpcDecimal(), false);
        await TestSort("fundinggoal", x => x.FundingGoal.FromGrpcDecimal(), true);
        await TestSort("-currentfunding", x => x.CurrentFunding.FromGrpcDecimal(), false);
        await TestSort("currentfunding", x => x.CurrentFunding.FromGrpcDecimal(), true);

        async Task TestSort(string sort, Func<Drimstarter.ProjectService.Client.ProjectDto, object> property,
            bool ascending)
        {
            var reply = await Act(sort: sort);

            if (ascending)
            {
                reply.Projects.Select(property).Should().BeInAscendingOrder();
            }
            else
            {
                reply.Projects.Select(property).Should().BeInDescendingOrder();
            }
        }
    }

    [Fact]
    public async Task Should_throw_when_invalid_sort()
    {
        Func<Task> action = () => Act(sort: "invalid");

        await action.ShouldThrowValidationErrorsException("sort", "Invalid sort", "projects:validation:sort_invalid");
    }

    [Fact]
    public async Task Should_paginate()
    {
        const int maxPageSize = 5;

        var projects = FakeFactory.CreateProjects(_dbCategory!.Id, 10);
        await _fixture.Database.Save(projects);

        var page1Ids = projects
            .OrderByDescending(x => x.StartDate)
            .Take(maxPageSize)
            .Select(x => IdEncoding.Encode(x.Id))
            .ToList();
        var page2Ids = projects
            .OrderByDescending(x => x.StartDate)
            .Skip(maxPageSize)
            .Take(maxPageSize)
            .Select(x => IdEncoding.Encode(x.Id))
            .ToList();

        var page1 = await Act(maxPageSize: maxPageSize);
        page1.Projects.Should().HaveCount(maxPageSize);
        page1.Projects.Select(x => x.Id).Should().BeEquivalentTo(page1Ids);
        page1.NextPageToken.Should().NotBeNull();

        var page2 = await Act(pageToken: page1.NextPageToken, maxPageSize: maxPageSize);
        page2.Projects.Should().HaveCount(maxPageSize);
        page2.Projects.Select(x => x.Id).Should().BeEquivalentTo(page2Ids);
        page2.NextPageToken.Should().NotBeNull();

        var page3 = await Act(pageToken: page2.NextPageToken, maxPageSize: maxPageSize);
        page3.Projects.Should().HaveCount(0);
        page3.NextPageToken.Should().BeNull();
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData(" ")]
    public async Task Should_throw_when_invalid_pageToken(string token)
    {
        Func<Task> action = () => Act(pageToken: token);

        await action.ShouldThrowValidationErrorsException("pageToken", "Invalid page token",
            "projects:validation:page_token_invalid");
    }

    [Fact]
    public async Task Should_throw_when_pageToken_with_different_query_parameters()
    {
        var projects = FakeFactory.CreateProjects(_dbCategory!.Id, 10);
        await _fixture.Database.Save(projects);

        var page1 = await Act(maxPageSize: 5);

        Func<Task> action = () => Act(pageToken: page1.NextPageToken, sort: "-title");

        await action.ShouldThrowValidationErrorsException("pageToken", "Invalid page token",
            "projects:validation:page_token_invalid");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Should_throw_when_maxPageSize_invalid(int maxPageSize)
    {
        Func<Task> action = () => Act(maxPageSize: maxPageSize);

        await action.ShouldThrowValidationErrorsException("maxPageSize", "Invalid max page size",
            "projects:validation:max_page_size_invalid");
    }
}
