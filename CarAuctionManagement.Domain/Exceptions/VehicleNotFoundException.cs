namespace CarAuctionManagement.Domain.Exceptions;

/// <summary>
/// Thrown when a requested vehicle cannot be found.
/// </summary>
public class VehicleNotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the VehicleNotFoundException class.
    /// </summary>
    public VehicleNotFoundException(string message) : base(message) { }
}
