using Drimstarter.AccountService.Client;
using Drimstarter.ApiGateway.Features.Accounts.Models;
using Drimstarter.Common.Web.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.Accounts.Requests;

public static class CreateAccount
{
    public class Endpoint : IEndpoint
    {
        private const string Path = "/accounts";

        public void MapEndpoint(WebApplication app)
        {
            app.MapPost(Path, async Task<Created<AccountModel>> (
                Body body,
                AccountService.Client.Accounts.AccountsClient accountClient,
                CancellationToken cancellationToken) =>
            {
                var request = new CreateAccountRequest
                {
                    Name = body.Name,
                    Email = body.Email,
                    Password = body.Password,
                };
                var reply = await accountClient.CreateAccountAsync(request, cancellationToken: cancellationToken);
                var account = new AccountModel(reply.Account.Id, reply.Account.Name, reply.Account.Email);
                return TypedResults.Created($"{Path}/{account.Id}", account);
            });
        }
    }

    private record Body(string Name, string Email, string Password);
}
