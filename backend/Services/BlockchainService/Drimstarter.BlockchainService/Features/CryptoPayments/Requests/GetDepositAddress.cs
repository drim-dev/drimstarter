using Drimstarter.Common.Database;
using Drimstarter.Common.Validation.Extensions;
using Drimstarter.BlockchainService.Database;
using Drimstarter.BlockchainService.Domain;
using FluentValidation;
using MediatR;
using static Drimstarter.BlockchainService.Features.CryptoPayments.Errors.CryptoPaymentsValidationErrors;

namespace Drimstarter.BlockchainService.Features.CryptoPayments.Requests;

public static class GetDepositAddress
{
    public class RequestValidator : AbstractValidator<GetDepositAddressRequest>
    {
        public RequestValidator(BlockchainDbContext db)
        {
            RuleFor(x => x.User)
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

        public async Task<GetDepositAddressReply> Handle(GetDepositAddressRequest request, CancellationToken cancellationToken)
        {
            var address = new Address()
            {
                AddressId = _idFactory.Create(),
                // TODO: write tests
                UserId = Guid.NewGuid().ToString(),
            };

            _db.Addresses.Add(address);
            await _db.SaveChangesAsync(cancellationToken);

            return new() { Address = address.AddressId.ToString() };
        }
    }
}
