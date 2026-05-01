namespace CarAuctionManagement.Domain.Entities;

/// <summary>
/// Represents an auction for a specific vehicle.
/// </summary>
public class Auction
{
    /// <summary>
    /// The ID of the vehicle being auctioned.
    /// </summary>
    public string VehicleId { get; }

    /// <summary>
    /// Indicates whether the auction is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// The current highest bid amount.
    /// </summary>
    public decimal CurrentHighestBid { get; private set; }

    /// <summary>
    /// The name of the highest bidder (nullable).
    /// </summary>
    public string? HighestBidder { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Auction class.
    /// </summary>
    public Auction(string vehicleId, decimal startingBid)
    {
        VehicleId = vehicleId;
        IsActive = true;
        CurrentHighestBid = startingBid;
        HighestBidder = null;
    }

    /// <summary>
    /// Initializes a new instance of the Auction class (parameterless constructor for EF Core).
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    public Auction()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

    /// <summary>
    /// Places a new bid on the auction.
    /// </summary>
    /// <param name="bidder">The name of the bidder.</param>
    /// <param name="amount">The bid amount.</param>
    /// <exception cref="Domain.Exceptions.InvalidBidException">
    /// Thrown when the bid is invalid (amount <= current highest bid or bidder is null/empty).
    /// </exception>
    /// <exception cref="Domain.Exceptions.AuctionNotActiveException">
    /// Thrown when the auction is not active.
    /// </exception>
    public void PlaceBid(string bidder, decimal amount)
    {
        if (!IsActive)
        {
            throw new Exceptions.AuctionNotActiveException("The auction is not active.");
        }

        if (string.IsNullOrWhiteSpace(bidder))
        {
            throw new Exceptions.InvalidBidException("Bidder cannot be null or empty.");
        }

        if (amount <= CurrentHighestBid)
        {
            throw new Exceptions.InvalidBidException($"Bid amount must be greater than the current highest bid of {CurrentHighestBid}.");
        }

        CurrentHighestBid = amount;
        HighestBidder = bidder;
    }

    /// <summary>
    /// Closes the auction.
    /// </summary>
    /// <exception cref="Domain.Exceptions.AuctionNotActiveException">
    /// Thrown when the auction is not active.
    /// </exception>
    public void Close()
    {
        if (!IsActive)
        {
            throw new Exceptions.AuctionNotActiveException("The auction is already closed.");
        }

        IsActive = false;
    }
}
