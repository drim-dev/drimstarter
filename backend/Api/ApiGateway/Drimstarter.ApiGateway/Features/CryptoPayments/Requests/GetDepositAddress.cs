using Drimstarter.ApiGateway.Features.CryptoPayments.Models;
using Drimstarter.BlockchainService.Client;
using Drimstarter.Common.Web.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.CryptoPayments.Requests;

public static class GetDepositAddress
{
    public class Endpoint : IEndpoint
    {
        private const string Path = "/crypto-payments/addresses";

        public void MapEndpoint(WebApplication app)
        {
            app.MapGet(Path, async Task<Ok<DepositAddressModel>> (
                Blockchain.BlockchainClient cryptoPaymentsClient,
                CancellationToken cancellationToken) =>
            {
                var request = new GetDepositAddressRequest
                {
                    UserId = "UserLoginOrId",
                };
                var reply = await cryptoPaymentsClient.GetDepositAddressAsync(request, cancellationToken: cancellationToken);
                var depositAddress = new DepositAddressModel(reply.Address);

                return TypedResults.Ok(depositAddress);
            });
        }
    }
}
