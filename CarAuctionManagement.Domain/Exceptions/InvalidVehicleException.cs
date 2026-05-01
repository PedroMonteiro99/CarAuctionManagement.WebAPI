namespace CarAuctionManagement.Domain.Exceptions;

/// <summary>
/// Thrown when a vehicle is invalid (null, missing required properties, or invalid values).
/// </summary>
public class InvalidVehicleException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the InvalidVehicleException class.
    /// </summary>
    public InvalidVehicleException(string message) : base(message) { }
}
