using Drimstarter.ApiGateway.Features.Categories.Models;
using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ProjectService.Client;
using MediatR;
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
            app.MapGet(Path, async Task<Ok<CategoryModel[]>>
                (ISender sender, CancellationToken cancellationToken) =>
            {
                var request = new Request();
                var categories = await sender.Send(request, cancellationToken);
                return TypedResults.Ok(categories);
            });
        }
    }

    public record Request : IRequest<CategoryModel[]>;

    public class RequestHandler : IRequestHandler<Request, CategoryModel[]>
    {
        private readonly ProjectService.Client.Categories.CategoriesClient _categoryClient;

        public RequestHandler(ProjectService.Client.Categories.CategoriesClient categoryClient)
        {
            _categoryClient = categoryClient;
        }

        public async Task<CategoryModel[]> Handle(Request request, CancellationToken cancellationToken)
        {
            var grpcRequest = new ListCategoriesRequest();
            var reply = await _categoryClient.ListCategoriesAsync(grpcRequest, cancellationToken: cancellationToken);
            return reply.Categories.Select(x => new CategoryModel(x.Id, x.Name)).ToArray();
        }
    }
}
