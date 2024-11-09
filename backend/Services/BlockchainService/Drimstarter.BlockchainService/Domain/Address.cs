namespace Drimstarter.BlockchainService.Domain;

public class Address
{
    public long Id { get; init; }

    public required string BitcoinAddress { get; init; }

    public required string UserId { get; init; }
}
