using Drimstarter.Common.Database;
using Drimstarter.Common.Validation.Extensions;
using Drimstarter.BlockchainService.Database;
using Drimstarter.BlockchainService.Domain;
using FluentValidation;
using MediatR;
using NBitcoin;
using static Drimstarter.BlockchainService.Features.CryptoPayments.Errors.CryptoPaymentsValidationErrors;

namespace Drimstarter.BlockchainService.Features.CryptoPayments.Requests;

public static class GetDepositAddress
{
    public class RequestValidator : AbstractValidator<GetDepositAddressRequest>
    {
        public RequestValidator(BlockchainDbContext db)
        {
            RuleFor(x => x.UserId)
                .NotEmpty(UserIdEmpty);
        }
    }

    public class RequestHandler : IRequestHandler<GetDepositAddressRequest, GetDepositAddressReply>
    {
        private readonly BlockchainDbContext _db;
        private readonly IdFactory _idFactory;

        public RequestHandler(
            BlockchainDbContext db,
            IdFactory idFactory)
        {
            _db = db;
            _idFactory = idFactory;
        }

        public async Task<GetDepositAddressReply> Handle(GetDepositAddressRequest request,
            CancellationToken cancellationToken)
        {
            var bitcoinAddress = new Key().PubKey.GetAddress(ScriptPubKeyType.Segwit, Network.Main).ToString();

            var address = new Address()
            {
                Id = _idFactory.Create(),
                UserId = request.UserId,
                BitcoinAddress = bitcoinAddress
            };

            _db.Addresses.Add(address);
            await _db.SaveChangesAsync(cancellationToken);

            return new GetDepositAddressReply { Address = address.BitcoinAddress };
        }
    }
}
