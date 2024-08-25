using Grpc.Core;
using MediatR;

namespace Drimstarter.ProjectService.Features.Categories;

public class CategoryApi(ISender _sender) : ProjectService.Categories.CategoriesBase
{
    public override Task<CreateCategoryReply> CreateCategory(CreateCategoryRequest request, ServerCallContext context)
    {
        return _sender.Send(request, context.CancellationToken);
    }

    public override Task<ListCategoriesReply> ListCategories(ListCategoriesRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);
}
