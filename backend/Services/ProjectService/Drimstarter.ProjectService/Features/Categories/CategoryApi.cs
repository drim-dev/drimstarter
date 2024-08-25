using Grpc.Core;
using MediatR;

namespace Drimstarter.ProjectService.Features.Categories;

public class CategoryApi : ProjectService.Categories.CategoriesBase
{
    private readonly ISender _sender;

    public CategoryApi(ISender sender)
    {
        _sender = sender;
    }

    public override Task<CreateCategoryReply> CreateCategory(CreateCategoryRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);

    public override Task<ListCategoriesReply> ListCategories(ListCategoriesRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);
}
