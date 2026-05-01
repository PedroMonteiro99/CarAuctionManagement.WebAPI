namespace CarAuctionManagement.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;
using CarAuctionManagement.Domain.Entities;

/// <summary>
/// Entity Framework Core DbContext for the Car Auction Management system.
/// </summary>
public class AuctionDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the AuctionDbContext class.
    /// </summary>
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Vehicles DbSet.
    /// </summary>
    public DbSet<Vehicle> Vehicles { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Auctions DbSet.
    /// </summary>
    public DbSet<Auction> Auctions { get; set; } = null!;

    /// <summary>
    /// Configures the model using the Fluent API.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Vehicle entity and its derived types (TPH - Table Per Hierarchy)
        var vehicleEntity = modelBuilder.Entity<Vehicle>();
        vehicleEntity.HasKey(v => v.Id);
        vehicleEntity.HasDiscriminator<string>("VehicleType")
            .HasValue<Sedan>("Sedan")
            .HasValue<Hatchback>("Hatchback")
            .HasValue<Suv>("SUV")
            .HasValue<Truck>("Truck");

        vehicleEntity.Property(v => v.Id)
            .HasMaxLength(100)
            .IsRequired();

        vehicleEntity.Property(v => v.Manufacturer)
            .HasMaxLength(100)
            .IsRequired();

        vehicleEntity.Property(v => v.Model)
            .HasMaxLength(100)
            .IsRequired();

        vehicleEntity.Property(v => v.Year)
            .IsRequired();

        vehicleEntity.Property(v => v.StartingBid)
            .HasPrecision(18, 2)
            .IsRequired();

        // Configure derived types (TPH)
        modelBuilder.Entity<Sedan>()
            .Property(s => s.NumberOfDoors)
            .IsRequired();

        modelBuilder.Entity<Hatchback>()
            .Property(h => h.NumberOfDoors)
            .IsRequired();

        modelBuilder.Entity<Suv>()
            .Property(s => s.NumberOfSeats)
            .IsRequired();

        modelBuilder.Entity<Truck>()
            .Property(t => t.LoadCapacity)
            .HasPrecision(18, 2)
            .IsRequired();

        // Configure Auction entity
        modelBuilder.Entity<Auction>()
            .HasKey(a => a.VehicleId);

        modelBuilder.Entity<Auction>()
            .Property(a => a.VehicleId)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Auction>()
            .Property(a => a.IsActive)
            .IsRequired();

        modelBuilder.Entity<Auction>()
            .Property(a => a.CurrentHighestBid)
            .HasPrecision(18, 2)
            .IsRequired();

        modelBuilder.Entity<Auction>()
            .Property(a => a.HighestBidder)
            .HasMaxLength(100);

        // Foreign key relationship
        modelBuilder.Entity<Auction>()
            .HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
