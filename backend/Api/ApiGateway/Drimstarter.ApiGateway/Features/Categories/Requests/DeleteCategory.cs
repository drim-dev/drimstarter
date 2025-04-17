using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ProjectService.Client;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Categories.Requests;

public static class DeleteCategory
{
    public class Endpoint : IEndpoint
    {
        private const string Path = "/categories";
        
        public void MapEndpoint(WebApplication app)
        {
            app.MapDelete(Path, async Task<NoContent> (
                Body body,
                ProjectService.Client.Categories.CategoriesClient categoryClient,
                CancellationToken cancellationToken) =>
            {
                var request = new DeleteCategoryRequest()
                {
                    Id = body.Id
                };
                await categoryClient.DeleteCategoryAsync(request, cancellationToken: cancellationToken);
                return TypedResults.NoContent();
            });
        }
    }
    
    private record Body(string Id);
}
