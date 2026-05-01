namespace CarAuctionManagementAPI.Controllers.Requests;

/// <summary>
/// Request model for placing a bid on an auction.
/// </summary>
public class PlaceBidRequest
{
    /// <summary>
    /// Gets or sets the ID of the vehicle being auctioned.
    /// </summary>
    public string VehicleId { get; set; }

    /// <summary>
    /// Gets or sets the bid amount.
    /// </summary>
    public decimal Amount { get; set; }
}