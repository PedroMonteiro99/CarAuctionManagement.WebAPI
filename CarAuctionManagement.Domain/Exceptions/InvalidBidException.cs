namespace CarAuctionManagement.Domain.Exceptions;

/// <summary>
/// Thrown when a bid is invalid.
/// </summary>
public class InvalidBidException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the InvalidBidException class.
    /// </summary>
    public InvalidBidException(string message) : base(message) { }
}
