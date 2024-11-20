using Drimstarter.ApiGateway.Features.Projects.Models;
using Drimstarter.Common.Grpc.Shared.Utils;
using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ProjectService.Client;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Projects.Requests;

public static class ListProjectsPageToken
{
    private const string Path = "/projects/page-token";

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet(Path, async Task<Ok<PageTokenResponse<ProjectModel>>> (
                string? categoryId,
                string? sort,
                string? pageToken,
                int? maxPageSize,
                ProjectService.Client.Projects.ProjectsClient projectClient,
                CancellationToken cancellationToken) =>
            {
                var grpcRequest = new ListProjectsPageTokenRequest
                {
                    CategoryId = categoryId,
                    Sort = sort,
                    PageToken = pageToken,
                    MaxPageSize = maxPageSize,
                };

                var reply = await projectClient.ListProjectsPageTokenAsync(grpcRequest, cancellationToken: cancellationToken);

                var projects = reply.Projects
                    .Select(x => new ProjectModel(x.Id, x.Title, x.Description, x.Story, x.FundingGoal.FromGrpcDecimal(),
                        x.CurrentFunding.FromGrpcDecimal(), x.StartDate.ToDateTime(), x.EndDate.ToDateTime()))
                    .ToArray();

                return TypedResults.Ok(new PageTokenResponse<ProjectModel>(projects, reply.NextPageToken));
            });
        }
    }
}
