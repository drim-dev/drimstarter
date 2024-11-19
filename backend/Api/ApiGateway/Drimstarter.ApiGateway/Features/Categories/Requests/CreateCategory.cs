using Drimstarter.ApiGateway.Features.Categories.Models;
using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ProjectService.Client;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Categories.Requests;

public static class CreateCategory
{
    public class Endpoint : IEndpoint
    {
        private const string Path = "/categories";

        public void MapEndpoint(WebApplication app)
        {
            app.MapPost(Path, async Task<Created<CategoryModel>> (
                Body body,
                ProjectService.Client.Categories.CategoriesClient categoryClient,
                CancellationToken cancellationToken) =>
            {
                var request = new CreateCategoryRequest
                {
                    Name = body.Name
                };
                var reply = await categoryClient.CreateCategoryAsync(request, cancellationToken: cancellationToken);
                var category = new CategoryModel(reply.Category.Id, reply.Category.Name);

                return TypedResults.Created($"{Path}/{category.Id}", category);
            });
        }
    }

    private record Body(string Name);
}
