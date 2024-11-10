using MediatR;

namespace Drimstarter.BlockchainService;

public partial class GetDepositAddressRequest : IRequest<GetDepositAddressReply>;
