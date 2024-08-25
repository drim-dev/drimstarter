using Drimstarter.Common.Tests.Grpc.Harnesses.Service.Base;
using Drimstarter.ProjectService.Features.Categories;
using Drimstarter.ServiceDefaults;
using Microsoft.AspNetCore.Builder;

namespace Drimstarter.ProjectService.Harnesses;

public class ProjectServiceHarness<TProgram> : BaseServiceHarness<TProgram>
    where TProgram : class
{
    public ProjectServiceHarness() : base(ResourceNames.ProjectService)
    {
    }

    protected override void MapGrpcServices(WebApplication webApplication)
    {
        webApplication.MapGrpcService<CategoryApi>();
    }
}
