namespace Drimstarter.BlockchainService.Domain;

public class Address
{
    public long AddressId { get; set; }

    public string BitcoinAddress { get; set; }

    public required string UserId { get; init; }
}
