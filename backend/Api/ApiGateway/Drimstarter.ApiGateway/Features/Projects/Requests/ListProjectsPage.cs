using Drimstarter.ApiGateway.Features.Projects.Models;
using Drimstarter.Common.Grpc.Shared.Utils;
using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ProjectService.Client;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Projects.Requests;

public class ListProjectsPage
{
    private const string Path = "/projects/page";

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet(Path, async Task<Ok<PageResponse<ProjectModel>>> (
                string? categoryId,
                string? sort,
                int? page,
                int? pageSize,
                ProjectService.Client.Projects.ProjectsClient projectClient,
                CancellationToken cancellationToken) =>
            {
                var grpcRequest = new ListProjectsPageRequest
                {
                    CategoryId = categoryId,
                    Sort = sort,
                    Page = page,
                    PageSize = pageSize,
                };

                var reply = await projectClient.ListProjectsPageAsync(grpcRequest, cancellationToken: cancellationToken);

                var projects = reply.Projects
                    .Select(x => new ProjectModel(x.Id, x.Title, x.Description, x.Story, x.FundingGoal.FromGrpcDecimal(),
                        x.CurrentFunding.FromGrpcDecimal(), x.StartDate.ToDateTime(), x.EndDate.ToDateTime()))
                    .ToArray();

                return TypedResults.Ok(new PageResponse<ProjectModel>(projects, reply.TotalItems));
            });
        }
    }
}
