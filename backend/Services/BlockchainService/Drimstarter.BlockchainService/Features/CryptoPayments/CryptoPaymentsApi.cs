using Grpc.Core;
using MediatR;

namespace Drimstarter.BlockchainService.Features.CryptoPayments;

public class CryptoPaymentsApi : BlockchainService.Blockchain.BlockchainBase
{
    private readonly ISender _sender;

    public CryptoPaymentsApi(ISender sender)
    {
        _sender = sender;
    }

    public override Task<GetDepositAddressReply> GetDepositAddress(GetDepositAddressRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);
}
