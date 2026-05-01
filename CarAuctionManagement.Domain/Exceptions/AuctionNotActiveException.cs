namespace CarAuctionManagement.Domain.Exceptions;

/// <summary>
/// Thrown when attempting to perform an action on an auction that is not active.
/// </summary>
public class AuctionNotActiveException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the AuctionNotActiveException class.
    /// </summary>
    public AuctionNotActiveException(string message) : base(message) { }
}
