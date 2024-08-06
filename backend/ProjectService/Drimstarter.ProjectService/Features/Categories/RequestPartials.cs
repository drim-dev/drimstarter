using MediatR;

namespace Drimstarter.ProjectService;

public partial class CreateCategoryRequest : IRequest<CreateCategoryReply>;

public partial class ListCategoriesRequest : IRequest<ListCategoriesReply>;
