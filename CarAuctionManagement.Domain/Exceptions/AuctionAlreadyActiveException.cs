namespace CarAuctionManagement.Domain.Exceptions;

/// <summary>
/// Thrown when attempting to start an auction for a vehicle that already has an active auction.
/// </summary>
public class AuctionAlreadyActiveException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the AuctionAlreadyActiveException class.
    /// </summary>
    public AuctionAlreadyActiveException(string message) : base(message) { }
}
