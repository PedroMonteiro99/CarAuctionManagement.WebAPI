namespace CarAuctionManagement.Domain.Ports;

using Entities;

/// <summary>
/// Port interface for auction repository operations.
/// </summary>
public interface IAuctionRepository
{
    /// <summary>
    /// Adds a new auction to the repository.
    /// </summary>
    void Add(Auction auction);

    /// <summary>
    /// Retrieves an auction by the vehicle ID.
    /// </summary>
    /// <returns>The auction if found; otherwise null.</returns>
    Auction? GetByVehicleId(string vehicleId);

    /// <summary>
    /// Updates an existing auction in the repository.
    /// </summary>
    void Update(Auction auction);
}
