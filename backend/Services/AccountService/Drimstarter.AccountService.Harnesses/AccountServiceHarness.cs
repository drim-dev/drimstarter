using Drimstarter.AccountService.Features.Accounts;
using Drimstarter.Common.Tests.Grpc.Harnesses.Service.Base;
using Drimstarter.ServiceDefaults;
using Microsoft.AspNetCore.Builder;

namespace Drimstarter.AccountService.Harnesses;

public class AccountServiceHarness<TProgram> : BaseServiceHarness<TProgram>
    where TProgram : class
{
    public AccountServiceHarness() : base(ResourceNames.AccountService)
    {
    }

    protected override void MapGrpcServices(WebApplication webApplication)
    {
        webApplication.MapGrpcService<AccountApi>();
    }
}
