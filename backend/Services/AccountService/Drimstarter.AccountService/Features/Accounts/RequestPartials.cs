using MediatR;

namespace Drimstarter.AccountService;

public partial class CreateAccountRequest : IRequest<CreateAccountReply>;
