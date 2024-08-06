using Drimstarter.ApiGateway.Features.Categories.Models;
using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ProjectService.Client;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Categories.Requests;

public static class CreateCategory
{
    public class Endpoint : IEndpoint
    {
        private const string Path = "/categories";

        public void MapEndpoint(WebApplication app)
        {
            app.MapPost(Path, async Task<Created<CategoryModel>>
                (ISender sender, Request body, CancellationToken cancellationToken) =>
            {
                var request = new Request(body.Name);
                var category = await sender.Send(request, cancellationToken);
                return TypedResults.Created($"{Path}/{category.Id}", category);
            });
        }
    }

    public record Request(string Name) : IRequest<CategoryModel>;

    public class RequestHandler(ProjectService.Client.Categories.CategoriesClient _categoryClient)
        : IRequestHandler<Request, CategoryModel>
    {
        public async Task<CategoryModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var grpcRequest = new CreateCategoryRequest { Name = request.Name };
            var reply = await _categoryClient.CreateCategoryAsync(grpcRequest, cancellationToken: cancellationToken);
            return new CategoryModel((short)reply.Category.Id, reply.Category.Name);
        }
    }
}
