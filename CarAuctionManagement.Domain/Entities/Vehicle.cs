namespace CarAuctionManagement.Domain.Entities;

/// <summary>
/// Abstract base class representing a vehicle in the auction system.
/// </summary>
public abstract class Vehicle
{
    /// <summary>
    /// Unique identifier for the vehicle.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Vehicle manufacturer name.
    /// </summary>
    public string Manufacturer { get; }

    /// <summary>
    /// Vehicle model name.
    /// </summary>
    public string Model { get; }

    /// <summary>
    /// Year of manufacture.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// Starting bid amount for the auction.
    /// </summary>
    public decimal StartingBid { get; }

    /// <summary>
    /// Initializes a new instance of the Vehicle class.
    /// </summary>
    protected Vehicle(string id, string manufacturer, string model, int year, decimal startingBid)
    {
        Id = id;
        Manufacturer = manufacturer;
        Model = model;
        Year = year;
        StartingBid = startingBid;
    }

    /// <summary>
    /// Initializes a new instance of the Vehicle class (parameterless constructor for EF Core).
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    protected Vehicle()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
}
