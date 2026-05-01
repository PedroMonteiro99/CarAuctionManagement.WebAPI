namespace CarAuctionManagement.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using CarAuctionManagement.Domain.Entities;
using CarAuctionManagement.Domain.Ports;
using Context;

/// <summary>
/// Entity Framework Core implementation of the auction repository.
/// </summary>
public class EfAuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;

    /// <summary>
    /// Initializes a new instance of the EfAuctionRepository class.
    /// </summary>
    public EfAuctionRepository(AuctionDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Adds a new auction to the repository.
    /// </summary>
    public void Add(Auction auction)
    {
        if (auction == null)
        {
            throw new ArgumentNullException(nameof(auction));
        }

        var existingAuction = _context.Auctions.FirstOrDefault(a => a.VehicleId == auction.VehicleId);

        if (existingAuction != null)
        {
            _context.Auctions.Remove(existingAuction);
        }

        _context.Auctions.Add(auction);

        _context.SaveChanges();
        
    }

    /// <summary>
    /// Retrieves an auction by vehicle ID.
    /// </summary>
    public Auction? GetByVehicleId(string vehicleId)
    {
        if (string.IsNullOrWhiteSpace(vehicleId))
        {
            return null;
        }

        return _context.Auctions.FirstOrDefault(a => a.VehicleId == vehicleId);
    }

    /// <summary>
    /// Updates an existing auction in the repository.
    /// </summary>
    public void Update(Auction auction)
    {
        if (auction == null)
        {
            throw new ArgumentNullException(nameof(auction));
        }

        _context.Auctions.Update(auction);
        _context.SaveChanges();
    }
}
