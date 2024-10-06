using Drimstarter.Common.Tests.Grpc.Harnesses.Service.Base;
using Drimstarter.BlockchainService.Features.CryptoPayments;
using Drimstarter.ServiceDefaults;
using Microsoft.AspNetCore.Builder;

namespace Drimstarter.BlockchainService.Harnesses;

public class BlockchainServiceHarness<TProgram> : BaseServiceHarness<TProgram>
    where TProgram : class
{
    public BlockchainServiceHarness() : base(ResourceNames.BlockchainService)
    {
    }

    protected override void MapGrpcServices(WebApplication webApplication)
    {
        webApplication.MapGrpcService<CryptoPaymentsApi>();
    }
}
