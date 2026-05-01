namespace CarAuctionManagement.Domain.Entities;

/// <summary>
/// Represents a truck vehicle type.
/// </summary>
public class Truck : Vehicle
{
    /// <summary>
    /// Load capacity of the truck in tons.
    /// </summary>
    public decimal LoadCapacity { get; }

    /// <summary>
    /// Initializes a new instance of the Truck class.
    /// </summary>
    public Truck(string id, string manufacturer, string model, int year, decimal startingBid, decimal loadCapacity)
        : base(id, manufacturer, model, year, startingBid)
    {
        LoadCapacity = loadCapacity;
    }

    /// <summary>
    /// Initializes a new instance of the Truck class (parameterless constructor for EF Core).
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    public Truck() : base()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
}
