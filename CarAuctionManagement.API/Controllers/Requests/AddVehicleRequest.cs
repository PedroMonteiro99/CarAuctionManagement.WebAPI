namespace CarAuctionManagementAPI.Controllers.Requests;

/// <summary>
/// Request model for adding a vehicle to the auction system.
/// </summary>
public class AddVehicleRequest
{
    /// <summary>
    /// Gets or sets the unique identifier for the vehicle.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the vehicle type (e.g., "Sedan", "Hatchback", "Suv", "Truck").
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

    /// <summary>
    /// Gets or sets the number of doors (for Sedan or Hatchback).
    /// </summary>
    public int? NumberOfDoors { get; set; }

    /// <summary>
    /// Gets or sets the number of seats (for SUV).
    /// </summary>
    public int? NumberOfSeats { get; set; }

    /// <summary>
    /// Gets or sets the load capacity in tons (for Truck).
    /// </summary>
    public decimal? LoadCapacity { get; set; }
}
