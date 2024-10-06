using Drimstarter.ApiGateway.Features.CryptoPayments.Models;
using Drimstarter.BlockchainService.Client;
using Drimstarter.Common.Web.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Drimstarter.ApiGateway.Features.CryptoPayments.Requests;

public static class GetDepositAddress
{
    public class Endpoint : IEndpoint
    {
        private const string Path = "/cryptopayments";

        public void MapEndpoint(WebApplication app)
        {
            app.MapPost(Path, async Task<Created<DepositAddressModel>> (
                Body body,
                Blockchain.BlockchainClient cryptoPaymentsClient,
                CancellationToken cancellationToken) =>
            {
                var request = new GetDepositAddressRequest
                {
                    User = "1",
                };
                var reply = await cryptoPaymentsClient.GetDepositAddressAsync(request, cancellationToken: cancellationToken);
                var depositAddress = new DepositAddressModel(reply.Address);
                return TypedResults.Created($"{Path}/{depositAddress.AddressId}", depositAddress);
            });
        }
    }

    private record Body(string Name);
}
