namespace CarAuctionManagement.Domain.Exceptions;

/// <summary>
/// Thrown when attempting to add a vehicle with an ID that already exists.
/// </summary>
public class DuplicateVehicleException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the DuplicateVehicleException class.
    /// </summary>
    public DuplicateVehicleException(string message) : base(message) { }
}
