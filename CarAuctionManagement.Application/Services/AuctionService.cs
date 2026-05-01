namespace CarAuctionManagement.Application.Services;

using Domain.Entities;
using Domain.Exceptions;
using Domain.Ports;

/// <summary>
/// Application service for managing auction operations.
/// </summary>
public class AuctionService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAuctionRepository _auctionRepository;
    private const decimal MinimumBidIncrement = 100m;

    /// <summary>
    /// Initializes a new instance of the AuctionService class.
    /// </summary>
    public AuctionService(
        IVehicleRepository vehicleRepository,
        IAuctionRepository auctionRepository)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository));
    }

    /// <summary>
    /// Adds a new vehicle to the system.
    /// </summary>
    /// <param name="vehicle">The vehicle to add.</param>
    /// <exception cref="InvalidVehicleException">
    /// Thrown when the vehicle is invalid or has invalid properties.
    /// </exception>
    /// <exception cref="DuplicateVehicleException">
    /// Thrown when a vehicle with the same ID already exists.
    /// </exception>
    public void AddVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
        {
            throw new InvalidVehicleException("Vehicle cannot be null.");
        }

        var existingVehicle = _vehicleRepository.GetById(vehicle.Id);
        if (existingVehicle != null)
        {
            throw new DuplicateVehicleException($"A vehicle with ID '{vehicle.Id}' already exists.");
        }

        _vehicleRepository.Add(vehicle);
    }

    /// <summary>
    /// Searches for vehicles based on optional criteria.
    /// </summary>
    /// <param name="type">The vehicle type (case-insensitive), optional.</param>
    /// <param name="manufacturer">The manufacturer name (case-insensitive), optional.</param>
    /// <param name="model">The model name (case-insensitive), optional.</param>
    /// <param name="year">The year of manufacture, optional.</param>
    /// <returns>An enumerable of matching vehicles.</returns>
    public IEnumerable<Vehicle> SearchVehicles(string? type, string? manufacturer, string? model, int? year)
    {
        return _vehicleRepository.Search(type, manufacturer, model, year);
    }

    /// <summary>
    /// Searches for vehicles based on optional criteria with pagination.
    /// </summary>
    /// <param name="type">The vehicle type (case-insensitive), optional.</param>
    /// <param name="manufacturer">The manufacturer name (case-insensitive), optional.</param>
    /// <param name="model">The model name (case-insensitive), optional.</param>
    /// <param name="year">The year of manufacture, optional.</param>
    /// <param name="pageNumber">The page number (1-based), default 1.</param>
    /// <param name="pageSize">The page size, default 10.</param>
    /// <returns>A paginated result of matching vehicles.</returns>
    public DTOs.PagedResult<Vehicle> SearchVehiclesPaged(string? type, string? manufacturer, string? model, int? year, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1 || pageSize > 100)
            pageSize = 10;

        var allVehicles = _vehicleRepository.Search(type, manufacturer, model, year);
        var totalCount = allVehicles.Count();

        var items = allVehicles
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new DTOs.PagedResult<Vehicle>(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Starts a new auction for a vehicle.
    /// </summary>
    /// <param name="vehicleId">The ID of the vehicle to auction.</param>
    /// <exception cref="VehicleNotFoundException">
    /// Thrown when the vehicle does not exist.
    /// </exception>
    /// <exception cref="AuctionAlreadyActiveException">
    /// Thrown when an active auction already exists for the vehicle.
    /// </exception>
    public void StartAuction(string vehicleId)
    {
        var vehicle = _vehicleRepository.GetById(vehicleId);
        if (vehicle == null)
        {
            throw new VehicleNotFoundException($"Vehicle with ID '{vehicleId}' not found.");
        }

        var existingAuction = _auctionRepository.GetByVehicleId(vehicleId);
        if (existingAuction != null && existingAuction.IsActive)
        {
            throw new AuctionAlreadyActiveException($"An active auction already exists for vehicle '{vehicleId}'.");
        }

        var auction = new Auction(vehicleId, vehicle.StartingBid);
        _auctionRepository.Add(auction);
    }

    /// <summary>
    /// Places a bid on an active auction.
    /// </summary>
    /// <param name="vehicleId">The ID of the vehicle being auctioned.</param>
    /// <param name="bidder">The name of the bidder.</param>
    /// <param name="amount">The bid amount.</param>
    /// <exception cref="VehicleNotFoundException">
    /// Thrown when the vehicle does not exist.
    /// </exception>
    /// <exception cref="AuctionNotActiveException">
    /// Thrown when there is no active auction for the vehicle.
    /// </exception>
    /// <exception cref="InvalidBidException">
    /// Thrown when the bid is invalid.
    /// </exception>
    public void PlaceBid(string vehicleId, string bidder, decimal amount)
    {
        var vehicle = _vehicleRepository.GetById(vehicleId);
        if (vehicle == null)
        {
            throw new VehicleNotFoundException($"Vehicle with ID '{vehicleId}' not found.");
        }

        var auction = _auctionRepository.GetByVehicleId(vehicleId);
        if (auction == null || !auction.IsActive)
        {
            throw new AuctionNotActiveException($"No active auction exists for vehicle '{vehicleId}'.");
        }

        // Validate minimum bid increment
        var minimumRequiredBid = auction.CurrentHighestBid + MinimumBidIncrement;
        if (amount < minimumRequiredBid)
        {
            throw new InvalidBidException($"Bid amount must be at least {minimumRequiredBid:C} (current highest bid + {MinimumBidIncrement:C}).");
        }

        auction.PlaceBid(bidder, amount);
        _auctionRepository.Update(auction);
    }

    /// <summary>
    /// Closes an active auction.
    /// </summary>
    /// <param name="vehicleId">The ID of the vehicle being auctioned.</param>
    /// <returns>The final state of the auction.</returns>
    /// <exception cref="VehicleNotFoundException">
    /// Thrown when the vehicle does not exist.
    /// </exception>
    /// <exception cref="AuctionNotActiveException">
    /// Thrown when there is no active auction for the vehicle.
    /// </exception>
    public Auction CloseAuction(string vehicleId)
    {
        var vehicle = _vehicleRepository.GetById(vehicleId);
        if (vehicle == null)
        {
            throw new VehicleNotFoundException($"Vehicle with ID '{vehicleId}' not found.");
        }

        var auction = _auctionRepository.GetByVehicleId(vehicleId);
        if (auction == null || !auction.IsActive)
        {
            throw new AuctionNotActiveException($"No active auction exists for vehicle '{vehicleId}'.");
        }

        auction.Close();
        _auctionRepository.Update(auction);

        return auction;
    }
}