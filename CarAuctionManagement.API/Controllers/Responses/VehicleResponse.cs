namespace CarAuctionManagementAPI.Controllers.Responses;

/// <summary>
/// Response model for vehicle information.
/// </summary>
public class VehicleResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the vehicle.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the vehicle type.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer name.
    /// </summary>
    public required string Manufacturer { get; set; }

    /// <summary>
    /// Gets or sets the model name.
    /// </summary>
    public required string Model { get; set; }

    /// <summary>
    /// Gets or sets the year of manufacture.
    /// </summary>
    public required int Year { get; set; }

    /// <summary>
    /// Gets or sets the starting bid amount.
    /// </summary>
    public required decimal StartingBid { get; set; }
}
