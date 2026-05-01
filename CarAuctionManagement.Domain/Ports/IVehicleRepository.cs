namespace CarAuctionManagement.Domain.Ports;

using Entities;

/// <summary>
/// Port interface for vehicle repository operations.
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Adds a vehicle to the repository.
    /// </summary>
    void Add(Vehicle vehicle);

    /// <summary>
    /// Retrieves a vehicle by its ID.
    /// </summary>
    /// <returns>The vehicle if found; otherwise null.</returns>
    Vehicle? GetById(string id);

    /// <summary>
    /// Searches for vehicles based on optional criteria.
    /// </summary>
    /// <param name="type">The vehicle type name (e.g., "Sedan", "SUV") - case-insensitive, optional.</param>
    /// <param name="manufacturer">The manufacturer name - case-insensitive, optional.</param>
    /// <param name="model">The model name - case-insensitive, optional.</param>
    /// <param name="year">The year of manufacture, optional.</param>
    /// <returns>An enumerable of matching vehicles.</returns>
    IEnumerable<Vehicle> Search(string? type, string? manufacturer, string? model, int? year);

    /// <summary>
    /// Retrieves all vehicles in the repository.
    /// </summary>
    IEnumerable<Vehicle> GetAll();
}
