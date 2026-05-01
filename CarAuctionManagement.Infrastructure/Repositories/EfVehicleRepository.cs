namespace CarAuctionManagement.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using CarAuctionManagement.Domain.Entities;
using CarAuctionManagement.Domain.Ports;
using Context;

/// <summary>
/// Entity Framework Core implementation of the vehicle repository.
/// </summary>
public class EfVehicleRepository : IVehicleRepository
{
    private readonly AuctionDbContext _context;
    private const int MaxSearchResults = 1000;

    /// <summary>
    /// Initializes a new instance of the EfVehicleRepository class.
    /// </summary>
    public EfVehicleRepository(AuctionDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Adds a vehicle to the repository.
    /// </summary>
    public void Add(Vehicle vehicle)
    {
        if (vehicle == null)
        {
            throw new ArgumentNullException(nameof(vehicle));
        }

        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }

    /// <summary>
    /// Retrieves a vehicle by its ID.
    /// </summary>
    public Vehicle? GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return _context.Vehicles.FirstOrDefault(v => v.Id == id);
    }

    /// <summary>
    /// Searches for vehicles based on optional criteria (case-insensitive).
    /// </summary>
    public IEnumerable<Vehicle> Search(string? type, string? manufacturer, string? model, int? year)
    {
        var query = _context.Vehicles.AsQueryable();

        // Filter by type at database level using OfType for derived types
        if (!string.IsNullOrWhiteSpace(type))
        {
            var vehicleType = type.Trim().ToLower();
            query = vehicleType switch
            {
                "sedan" => query.OfType<Sedan>(),
                "hatchback" => query.OfType<Hatchback>(),
                "suv" => query.OfType<Suv>(),
                "truck" => query.OfType<Truck>(),
                _ => query  // If type doesn't match any known type, return all
            };
        }

        // Apply server-side filters that can be translated by EF Core
        // Use EF Core's built-in case-insensitive comparison for better performance
        if (!string.IsNullOrWhiteSpace(manufacturer))
        {
            query = query.Where(v => EF.Functions.Like(v.Manufacturer, $"*{manufacturer}*"));
        }

        if (!string.IsNullOrWhiteSpace(model))
        {
            query = query.Where(v => EF.Functions.Like(v.Model, $"*{model}*"));
        }

        if (year.HasValue)
        {
            query = query.Where(v => v.Year == year.Value);
        }

        // Apply maximum result limit to prevent memory exhaustion
        query = query.Take(MaxSearchResults);

        // Materialize after all filters have been applied at database level
        return query.ToList();
    }

    /// <summary>
    /// Retrieves all vehicles in the repository.
    /// </summary>
    public IEnumerable<Vehicle> GetAll()
    {
        // Use AsNoTracking for read-only operations to improve performance
        return _context.Vehicles.AsNoTracking().ToList();
    }
}
