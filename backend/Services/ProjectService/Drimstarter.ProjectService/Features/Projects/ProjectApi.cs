using Grpc.Core;
using MediatR;

namespace Drimstarter.ProjectService.Features.Projects;

public class ProjectApi : ProjectService.Projects.ProjectsBase
{
    private readonly ISender _sender;

    public ProjectApi(ISender sender) => _sender = sender;

    public override Task<ListProjectsPageReply> ListProjectsPage(ListProjectsPageRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);

    public override Task<ListProjectsOffsetLimitReply> ListProjectsOffsetLimit(ListProjectsOffsetLimitRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);

    public override Task<ListProjectsKeysetReply> ListProjectsKeyset(ListProjectsKeysetRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);

    public override Task<ListProjectsPageTokenReply> ListProjectsPageToken(ListProjectsPageTokenRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);
}
