namespace CarAuctionManagementAPI.Controllers.Responses;

/// <summary>
/// Response model for auction information.
/// </summary>
public class AuctionResponse
{
    /// <summary>
    /// Gets or sets the ID of the vehicle being auctioned.
    /// </summary>
    public required string VehicleId { get; set; }

    /// <summary>
    /// Gets or sets the current highest bid amount.
    /// </summary>
    public required decimal CurrentHighestBid { get; set; }

    /// <summary>
    /// Gets or sets the name of the highest bidder.
    /// </summary>
    public string? HighestBidder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the auction is active.
    /// </summary>
    public required bool IsActive { get; set; }
}
