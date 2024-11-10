using Drimstarter.BlockchainService.Tests.Fixtures;

namespace Drimstarter.BlockchainService.Tests.Features.CryptoPayments;

[CollectionDefinition(Name)]
public class CryptoPaymentsTestsCollection : ICollectionFixture<TestFixture>
{
    public const string Name = nameof(CryptoPaymentsTestsCollection);
}
