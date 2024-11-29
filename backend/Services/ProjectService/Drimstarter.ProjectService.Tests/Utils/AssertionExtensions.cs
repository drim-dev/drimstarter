using Drimstarter.Common.Grpc.Shared.Utils;
using Drimstarter.ProjectService.Domain;
using FluentAssertions;

namespace Drimstarter.ProjectService.Tests.Utils;

public static class AssertionExtensions
{
    public static void ShouldBeEquivalentTo(this Client.ProjectDto projectDto, Project project)
    {
        projectDto.Title.Should().Be(project.Title);
        projectDto.Description.Should().Be(project.Description);
        projectDto.Story.Should().Be(project.Story);
        projectDto.FundingGoal.FromGrpcDecimal().Should().Be(project.FundingGoal);
        projectDto.CurrentFunding.FromGrpcDecimal().Should().Be(project.CurrentFunding);
        projectDto.StartDate.ToDateTime().Should().BeCloseTo(project.StartDate, TimeSpan.FromSeconds(1));
        projectDto.EndDate.ToDateTime().Should().BeCloseTo(project.EndDate, TimeSpan.FromSeconds(1));
    }
}
