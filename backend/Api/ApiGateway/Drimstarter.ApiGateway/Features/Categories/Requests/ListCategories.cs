using Drimstarter.ApiGateway.Features.Categories.Models;
using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ProjectService.Client;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Categories.Requests;

public static class ListCategories
{
    public class Endpoint : IEndpoint
    {
        // TODO: think about using a constant for the path
        private const string Path = "/categories";

        public void MapEndpoint(WebApplication app)
        {
            app.MapGet(Path, async Task<Ok<CategoryModel[]>> (
                ProjectService.Client.Categories.CategoriesClient categoryClient,
                CancellationToken cancellationToken) =>
            {
                var grpcRequest = new ListCategoriesRequest();
                var reply = await categoryClient.ListCategoriesAsync(grpcRequest, cancellationToken: cancellationToken);
                var categories = reply.Categories.Select(x => new CategoryModel(x.Id, x.Name)).ToArray();

                return TypedResults.Ok(categories);
            });
        }
    }
}
