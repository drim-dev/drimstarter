using Drimstarter.AccountService.Tests.Fixtures;

namespace Drimstarter.AccountService.Tests.Features.Accounts;

[CollectionDefinition(Name)]
public class AccountTestsCollection : ICollectionFixture<TestFixture>
{
    public const string Name = nameof(AccountTestsCollection);
}
