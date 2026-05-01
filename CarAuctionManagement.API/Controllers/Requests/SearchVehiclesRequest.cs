namespace CarAuctionManagementAPI.Controllers.Requests;

/// <summary>
/// Request model for searching vehicles.
/// </summary>
public class SearchVehiclesRequest
{
    /// <summary>
    /// Gets or sets the vehicle type filter (optional, case-insensitive).
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer name filter (optional, case-insensitive).
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// Gets or sets the model name filter (optional, case-insensitive).
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Gets or sets the year filter (optional).
    /// </summary>
    public int? Year { get; set; }
}
