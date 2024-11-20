using Drimstarter.ApiGateway.Features.Projects.Models;
using Drimstarter.Common.Grpc.Shared.Utils;
using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ProjectService.Client;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Projects.Requests;

public static class ListProjectsOffsetLimit
{
    private const string Path = "/projects/offset-limit";

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet(Path, async Task<Ok<OffsetLimitResponse<ProjectModel>>> (
                string? categoryId,
                string? sort,
                int? offset,
                int? limit,
                ProjectService.Client.Projects.ProjectsClient projectClient,
                CancellationToken cancellationToken) =>
            {
                var grpcRequest = new ListProjectsOffsetLimitRequest
                {
                    CategoryId = categoryId,
                    Sort = sort,
                    Offset = offset,
                    Limit = limit,
                };

                var reply = await projectClient.ListProjectsOffsetLimitAsync(grpcRequest, cancellationToken: cancellationToken);

                var projects = reply.Projects
                    .Select(x => new ProjectModel(x.Id, x.Title, x.Description, x.Story, x.FundingGoal.FromGrpcDecimal(),
                        x.CurrentFunding.FromGrpcDecimal(), x.StartDate.ToDateTime(), x.EndDate.ToDateTime()))
                    .ToArray();

                return TypedResults.Ok(new OffsetLimitResponse<ProjectModel>(projects, reply.HasMore));
            });
        }
    }
}
