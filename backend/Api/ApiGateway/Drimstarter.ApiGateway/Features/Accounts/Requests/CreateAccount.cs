using Drimstarter.AccountService.Client;
using Drimstarter.ApiGateway.Features.Accounts.Models;
using Drimstarter.Common.Web.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Accounts.Requests;

public static class CreateAccount
{
    public class Endpoint : IEndpoint
    {
        private const string Path = "/accounts";

        public void MapEndpoint(WebApplication app)
        {
            app.MapPost(Path, async Task<Created<AccountModel>>
                (ISender sender, Request body, CancellationToken cancellationToken) =>
            {
                var account = await sender.Send(body, cancellationToken);
                return TypedResults.Created($"{Path}/{account.Id}", account);
            });
        }
    }

    public record Request(string Name, string Email, string Password) : IRequest<AccountModel>;

    public class RequestHandler : IRequestHandler<Request, AccountModel>
    {
        private readonly AccountService.Client.Accounts.AccountsClient _accountClient;

        public RequestHandler(AccountService.Client.Accounts.AccountsClient accountClient)
        {
            _accountClient = accountClient;
        }

        public async Task<AccountModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var grpcRequest = new CreateAccountRequest
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
            };
            var reply = await _accountClient.CreateAccountAsync(grpcRequest, cancellationToken: cancellationToken);
            return new AccountModel(reply.Account.Id, reply.Account.Name, reply.Account.Email);
        }
    }
}
