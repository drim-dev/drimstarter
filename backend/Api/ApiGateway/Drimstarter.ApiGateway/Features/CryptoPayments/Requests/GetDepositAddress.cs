using Drimstarter.ApiGateway.Features.CryptoPayments.Models;
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
            // app.MapPost(Path, async Task<Created<DepositAddressModel>> (
            //     Body body,
            //     BlockchainService.Client.CryptoPaymentsClient cryptoPaymentsClient,
            //     CancellationToken cancellationToken) =>
            // {
            //     var request = new GetDepositAddressRequest
            //     {
            //         Name = body.Name
            //     };
            //     var reply = await cryptoPaymentsClient.GetDepositAddressAsync(request, cancellationToken: cancellationToken);
            //     var depositAddress = new DepositAddressModel(reply.AddressId);
            //     return TypedResults.Created($"{Path}/{depositAddress.AddressId}", depositAddress);
            // });
        }
    }

    private record Body(string Name);
}
