namespace CarAuctionManagement.Domain.Entities;

/// <summary>
/// Represents a sedan vehicle type.
/// </summary>
public class Sedan : Vehicle
{
    /// <summary>
    /// Number of doors on the sedan.
    /// </summary>
    public int NumberOfDoors { get; }

    /// <summary>
    /// Initializes a new instance of the Sedan class.
    /// </summary>
    public Sedan(string id, string manufacturer, string model, int year, decimal startingBid, int numberOfDoors)
        : base(id, manufacturer, model, year, startingBid)
    {
        NumberOfDoors = numberOfDoors;
    }

    /// <summary>
    /// Initializes a new instance of the Sedan class (parameterless constructor for EF Core).
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    public Sedan() : base()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
}
