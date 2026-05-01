namespace CarAuctionManagement.Domain.Entities;

/// <summary>
/// Represents a hatchback vehicle type.
/// </summary>
public class Hatchback : Vehicle
{
    /// <summary>
    /// Number of doors on the hatchback.
    /// </summary>
    public int NumberOfDoors { get; }

    /// <summary>
    /// Initializes a new instance of the Hatchback class.
    /// </summary>
    public Hatchback(string id, string manufacturer, string model, int year, decimal startingBid, int numberOfDoors)
        : base(id, manufacturer, model, year, startingBid)
    {
        NumberOfDoors = numberOfDoors;
    }

    /// <summary>
    /// Initializes a new instance of the Hatchback class (parameterless constructor for EF Core).
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    public Hatchback() : base()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
}
