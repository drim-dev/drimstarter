using Drimstarter.ApiGateway.Features.Projects.Models;
using Drimstarter.Common.Grpc.Shared.Utils;
using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ProjectService.Client;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Projects.Requests;

public static class ListProjectsKeyset
{
    private const string Path = "/projects/keyset";

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet(Path, async Task<Ok<KeysetResponse<ProjectModel>>> (
                string? categoryId,
                string? startAfter,
                int? pageSize,
                ProjectService.Client.Projects.ProjectsClient projectClient,
                CancellationToken cancellationToken) =>
            {
                var grpcRequest = new ListProjectsKeysetRequest
                {
                    CategoryId = categoryId,
                    StartAfter = startAfter,
                    PageSize = pageSize,
                };

                var reply = await projectClient.ListProjectsKeysetAsync(grpcRequest, cancellationToken: cancellationToken);

                var projects = reply.Projects
                    .Select(x => new ProjectModel(x.Id, x.Title, x.Description, x.Story, x.FundingGoal.FromGrpcDecimal(),
                        x.CurrentFunding.FromGrpcDecimal(), x.StartDate.ToDateTime(), x.EndDate.ToDateTime()))
                    .ToArray();

                return TypedResults.Ok(new KeysetResponse<ProjectModel>(projects, reply.NextStartAfter));
            });
        }
    }
}
