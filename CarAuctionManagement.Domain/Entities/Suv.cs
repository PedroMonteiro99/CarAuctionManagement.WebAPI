namespace CarAuctionManagement.Domain.Entities;

/// <summary>
/// Represents an SUV (Sport Utility Vehicle) type.
/// </summary>
public class Suv : Vehicle
{
    /// <summary>
    /// Number of seats in the SUV.
    /// </summary>
    public int NumberOfSeats { get; }

    /// <summary>
    /// Initializes a new instance of the Suv class.
    /// </summary>
    public Suv(string id, string manufacturer, string model, int year, decimal startingBid, int numberOfSeats)
        : base(id, manufacturer, model, year, startingBid)
    {
        NumberOfSeats = numberOfSeats;
    }

    /// <summary>
    /// Initializes a new instance of the Suv class (parameterless constructor for EF Core).
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    public Suv() : base()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
}
