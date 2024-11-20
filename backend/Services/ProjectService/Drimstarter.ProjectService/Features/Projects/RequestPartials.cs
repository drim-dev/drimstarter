using MediatR;

namespace Drimstarter.ProjectService;

public partial class ListProjectsPageRequest : IRequest<ListProjectsPageReply>;
public partial class ListProjectsOffsetLimitRequest : IRequest<ListProjectsOffsetLimitReply>;
public partial class ListProjectsKeysetRequest : IRequest<ListProjectsKeysetReply>;
public partial class ListProjectsPageTokenRequest : IRequest<ListProjectsPageTokenReply>;
