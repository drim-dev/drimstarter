using Grpc.Core;
using MediatR;

namespace Drimstarter.AccountService.Features.Accounts;

public class AccountApi : AccountService.Accounts.AccountsBase
{
    private readonly ISender _sender;

    public AccountApi(ISender sender)
    {
        _sender = sender;
    }

    public override Task<CreateAccountReply> CreateAccount(CreateAccountRequest request, ServerCallContext context) =>
        _sender.Send(request, context.CancellationToken);
}
