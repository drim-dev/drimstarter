using Grpc.Core;
using MediatR;

namespace Drimstarter.ProjectService.Features.Projects;

public class ProjectApi : ProjectService.Projects.ProjectsBase
{
    private readonly ISender _sender;

    public ProjectApi(ISender sender)
    {
        _sender = sender;
    }

    public override Task<ListProjectsReply> ListProjects(ListProjectsRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);
}
