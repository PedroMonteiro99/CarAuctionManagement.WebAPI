namespace CarAuctionManagement.Tests;

using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using CarAuctionManagement.Application.Services;
using CarAuctionManagement.Domain.Entities;
using CarAuctionManagement.Domain.Exceptions;
using CarAuctionManagement.Domain.Ports;
using CarAuctionManagement.Infrastructure.Context;
using CarAuctionManagement.Infrastructure.Repositories;

/// <summary>
/// Unit tests for the AuctionService class.
/// </summary>
public class AuctionServiceTests : IDisposable
{
    private readonly AuctionService _auctionService;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAuctionRepository _auctionRepository;
    private readonly AuctionDbContext _context;

    public AuctionServiceTests()
    {
        var options = new DbContextOptionsBuilder<AuctionDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AuctionDbContext(options);
        _vehicleRepository = new EfVehicleRepository(_context);
        _auctionRepository = new EfAuctionRepository(_context);
        _auctionService = new AuctionService(_vehicleRepository, _auctionRepository);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    #region AddVehicle Tests

    [Fact]
    public void AddVehicle_WithValidSedan_ShouldSucceed()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);

        // Act
        _auctionService.AddVehicle(vehicle);

        // Assert
        var retrieved = _vehicleRepository.GetById("sedan-001");
        retrieved.Should().NotBeNull();
        retrieved.Should().BeOfType<Sedan>();
        retrieved!.Manufacturer.Should().Be("Toyota");
    }

    [Fact]
    public void AddVehicle_WithValidHatchback_ShouldSucceed()
    {
        // Arrange
        var vehicle = new Hatchback("hatch-001", "Honda", "Civic", 2022, 12000m, 3);

        // Act
        _auctionService.AddVehicle(vehicle);

        // Assert
        var retrieved = _vehicleRepository.GetById("hatch-001");
        retrieved.Should().NotBeNull();
        retrieved.Should().BeOfType<Hatchback>();
    }

    [Fact]
    public void AddVehicle_WithValidSuv_ShouldSucceed()
    {
        // Arrange
        var vehicle = new Suv("suv-001", "Ford", "Explorer", 2021, 30000m, 7);

        // Act
        _auctionService.AddVehicle(vehicle);

        // Assert
        var retrieved = _vehicleRepository.GetById("suv-001");
        retrieved.Should().NotBeNull();
        retrieved.Should().BeOfType<Suv>();
    }

    [Fact]
    public void AddVehicle_WithValidTruck_ShouldSucceed()
    {
        // Arrange
        var vehicle = new Truck("truck-001", "Volvo", "FH16", 2020, 50000m, 25.5m);

        // Act
        _auctionService.AddVehicle(vehicle);

        // Assert
        var retrieved = _vehicleRepository.GetById("truck-001");
        retrieved.Should().NotBeNull();
        retrieved.Should().BeOfType<Truck>();
    }

    [Fact]
    public void AddVehicle_WithDuplicateId_ShouldThrowDuplicateVehicleException()
    {
        // Arrange
        var vehicle1 = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        var vehicle2 = new Sedan("sedan-001", "Honda", "Accord", 2023, 18000m, 4);

        _auctionService.AddVehicle(vehicle1);

        // Act & Assert
        var ex = Assert.Throws<DuplicateVehicleException>(() => _auctionService.AddVehicle(vehicle2));
        ex.Message.Should().Contain("sedan-001");
    }

    [Fact]
    public void AddVehicle_WithNullVehicle_ShouldThrowInvalidVehicleException()
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidVehicleException>(() => _auctionService.AddVehicle(null!));
        ex.Message.Should().Contain("null");
    }

    [Fact]
    public void AddVehicle_WithNullId_ShouldThrowInvalidVehicleException()
    {
        // Arrange
        var vehicle = new Sedan(null!, "Toyota", "Camry", 2023, 15000m, 4);

        // Act & Assert
        var ex = Assert.Throws<InvalidVehicleException>(() => _auctionService.AddVehicle(vehicle));
        ex.Message.Should().Contain("ID");
    }

    [Fact]
    public void AddVehicle_WithEmptyId_ShouldThrowInvalidVehicleException()
    {
        // Arrange
        var vehicle = new Sedan("", "Toyota", "Camry", 2023, 15000m, 4);

        // Act & Assert
        var ex = Assert.Throws<InvalidVehicleException>(() => _auctionService.AddVehicle(vehicle));
        ex.Message.Should().Contain("ID");
    }

    [Fact]
    public void AddVehicle_WithYearTooOld_ShouldThrowInvalidVehicleException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 1885, 15000m, 4);

        // Act & Assert
        var ex = Assert.Throws<InvalidVehicleException>(() => _auctionService.AddVehicle(vehicle));
        ex.Message.Should().Contain("1885");
    }

    [Fact]
    public void AddVehicle_WithNegativeStartingBid_ShouldThrowInvalidVehicleException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, -1000m, 4);

        // Act & Assert
        var ex = Assert.Throws<InvalidVehicleException>(() => _auctionService.AddVehicle(vehicle));
        ex.Message.Should().Contain("greater than 0");
    }

    [Fact]
    public void AddVehicle_WithZeroStartingBid_ShouldThrowInvalidVehicleException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 0m, 4);

        // Act & Assert
        var ex = Assert.Throws<InvalidVehicleException>(() => _auctionService.AddVehicle(vehicle));
        ex.Message.Should().Contain("greater than 0");
    }

    #endregion

    #region SearchVehicles Tests

    [Fact]
    public void SearchVehicles_WithNoFilter_ShouldReturnAllVehicles()
    {
        // Arrange
        var sedan = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        var suv = new Suv("suv-001", "Ford", "Explorer", 2021, 30000m, 7);
        _auctionService.AddVehicle(sedan);
        _auctionService.AddVehicle(suv);

        // Act
        var results = _auctionService.SearchVehicles(null, null, null, null);

        // Assert
        results.Should().HaveCount(2);
    }

    [Fact]
    public void SearchVehicles_FilterByType_ShouldReturnOnlyMatching()
    {
        // Arrange
        var sedan = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        var suv = new Suv("suv-001", "Ford", "Explorer", 2021, 30000m, 7);
        _auctionService.AddVehicle(sedan);
        _auctionService.AddVehicle(suv);

        // Act
        var results = _auctionService.SearchVehicles("Sedan", null, null, null);

        // Assert
        results.Should().HaveCount(1);
        results.First().Should().BeOfType<Sedan>();
    }

    [Fact]
    public void SearchVehicles_FilterByTypeIsCaseInsensitive()
    {
        // Arrange
        var sedan = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(sedan);

        // Act
        var results = _auctionService.SearchVehicles("sedan", null, null, null);

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public void SearchVehicles_FilterByManufacturer_ShouldReturnOnlyMatching()
    {
        // Arrange
        var sedan = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        var suv = new Suv("suv-001", "Ford", "Explorer", 2021, 30000m, 7);
        _auctionService.AddVehicle(sedan);
        _auctionService.AddVehicle(suv);

        // Act
        var results = _auctionService.SearchVehicles(null, "Toyota", null, null);

        // Assert
        results.Should().HaveCount(1);
        results.First().Manufacturer.Should().Be("Toyota");
    }

    [Fact]
    public void SearchVehicles_FilterByManufacturerIsCaseInsensitive()
    {
        // Arrange
        var sedan = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(sedan);

        // Act
        var results = _auctionService.SearchVehicles(null, "toyota", null, null);

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public void SearchVehicles_FilterByModel_ShouldReturnOnlyMatching()
    {
        // Arrange
        var sedan1 = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        var sedan2 = new Sedan("sedan-002", "Toyota", "Corolla", 2022, 12000m, 4);
        _auctionService.AddVehicle(sedan1);
        _auctionService.AddVehicle(sedan2);

        // Act
        var results = _auctionService.SearchVehicles(null, null, "Camry", null);

        // Assert
        results.Should().HaveCount(1);
        results.First().Model.Should().Be("Camry");
    }

    [Fact]
    public void SearchVehicles_FilterByModelIsCaseInsensitive()
    {
        // Arrange
        var sedan = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(sedan);

        // Act
        var results = _auctionService.SearchVehicles(null, null, "camry", null);

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public void SearchVehicles_FilterByYear_ShouldReturnOnlyMatching()
    {
        // Arrange
        var sedan1 = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        var sedan2 = new Sedan("sedan-002", "Toyota", "Corolla", 2022, 12000m, 4);
        _auctionService.AddVehicle(sedan1);
        _auctionService.AddVehicle(sedan2);

        // Act
        var results = _auctionService.SearchVehicles(null, null, null, 2023);

        // Assert
        results.Should().HaveCount(1);
        results.First().Year.Should().Be(2023);
    }

    [Fact]
    public void SearchVehicles_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var sedan = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(sedan);

        // Act
        var results = _auctionService.SearchVehicles("SUV", null, null, null);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void SearchVehicles_WithMultipleFilters_ShouldApplyAll()
    {
        // Arrange
        var sedan1 = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        var sedan2 = new Sedan("sedan-002", "Toyota", "Corolla", 2023, 12000m, 4);
        var sedan3 = new Sedan("sedan-003", "Honda", "Accord", 2023, 18000m, 4);
        _auctionService.AddVehicle(sedan1);
        _auctionService.AddVehicle(sedan2);
        _auctionService.AddVehicle(sedan3);

        // Act
        var results = _auctionService.SearchVehicles("Sedan", "Toyota", null, 2023);

        // Assert
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(v => v.Manufacturer.Should().Be("Toyota"));
    }

    #endregion

    #region StartAuction Tests

    [Fact]
    public void StartAuction_WithValidVehicle_ShouldSucceed()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);

        // Act
        _auctionService.StartAuction("sedan-001");

        // Assert
        var auction = _auctionRepository.GetByVehicleId("sedan-001");
        auction.Should().NotBeNull();
        auction!.IsActive.Should().BeTrue();
        auction.CurrentHighestBid.Should().Be(15000m);
    }

    [Fact]
    public void StartAuction_WithNonExistentVehicle_ShouldThrowVehicleNotFoundException()
    {
        // Act & Assert
        var ex = Assert.Throws<VehicleNotFoundException>(() => _auctionService.StartAuction("non-existent"));
        ex.Message.Should().Contain("not found");
    }

    [Fact]
    public void StartAuction_WhenAuctionAlreadyActive_ShouldThrowAuctionAlreadyActiveException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");

        // Act & Assert
        var ex = Assert.Throws<AuctionAlreadyActiveException>(() => _auctionService.StartAuction("sedan-001"));
        ex.Message.Should().Contain("already exists");
    }

    #endregion

    #region PlaceBid Tests

    [Fact]
    public void PlaceBid_WithValidBid_ShouldSucceed()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");

        // Act
        _auctionService.PlaceBid("sedan-001", "John", 16000m);

        // Assert
        var auction = _auctionRepository.GetByVehicleId("sedan-001");
        auction!.CurrentHighestBid.Should().Be(16000m);
        auction.HighestBidder.Should().Be("John");
    }

    [Fact]
    public void PlaceBid_WithNonExistentVehicle_ShouldThrowVehicleNotFoundException()
    {
        // Act & Assert
        var ex = Assert.Throws<VehicleNotFoundException>(() => _auctionService.PlaceBid("non-existent", "John", 20000m));
        ex.Message.Should().Contain("not found");
    }

    [Fact]
    public void PlaceBid_WhenNoActiveAuction_ShouldThrowAuctionNotActiveException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);

        // Act & Assert
        var ex = Assert.Throws<AuctionNotActiveException>(() => _auctionService.PlaceBid("sedan-001", "John", 20000m));
        ex.Message.Should().Contain("No active auction");
    }

    [Fact]
    public void PlaceBid_WithBidLessThanOrEqualToCurrentHighest_ShouldThrowInvalidBidException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");

        // Act & Assert
        var ex = Assert.Throws<InvalidBidException>(() => _auctionService.PlaceBid("sedan-001", "John", 15000m));
        ex.Message.Should().Contain("at least");
    }

    [Fact]
    public void PlaceBid_WithNullBidder_ShouldThrowInvalidBidException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");

        // Act & Assert
        var ex = Assert.Throws<InvalidBidException>(() => _auctionService.PlaceBid("sedan-001", null!, 20000m));
        ex.Message.Should().Contain("Bidder");
    }

    [Fact]
    public void PlaceBid_WithEmptyBidder_ShouldThrowInvalidBidException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");

        // Act & Assert
        var ex = Assert.Throws<InvalidBidException>(() => _auctionService.PlaceBid("sedan-001", "", 20000m));
        ex.Message.Should().Contain("Bidder");
    }

    [Fact]
    public void PlaceBid_MultipleValidBids_ShouldUpdateToHighest()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");

        // Act
        _auctionService.PlaceBid("sedan-001", "John", 16000m);
        _auctionService.PlaceBid("sedan-001", "Jane", 17000m);
        _auctionService.PlaceBid("sedan-001", "Bob", 18000m);

        // Assert
        var auction = _auctionRepository.GetByVehicleId("sedan-001");
        auction!.CurrentHighestBid.Should().Be(18000m);
        auction.HighestBidder.Should().Be("Bob");
    }

    [Fact]
    public void PlaceBid_WithLessThanMinimumIncrement_ShouldThrowInvalidBidException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");

        // Act & Assert - Bid must be at least CurrentBid (15000) + MinimumIncrement (100) = 15100
        var ex = Assert.Throws<InvalidBidException>(() => _auctionService.PlaceBid("sedan-001", "John", 15050m));
        ex.Message.Should().Contain("at least");
    }

    [Fact]
    public void PlaceBid_WithExactMinimumIncrement_ShouldSucceed()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");

        // Act - Bid exactly at minimum increment
        _auctionService.PlaceBid("sedan-001", "John", 15100m);

        // Assert
        var auction = _auctionRepository.GetByVehicleId("sedan-001");
        auction!.CurrentHighestBid.Should().Be(15100m);
    }

    #endregion

    #region CloseAuction Tests

    [Fact]
    public void CloseAuction_WithActiveAuction_ShouldSucceed()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");
        _auctionService.PlaceBid("sedan-001", "John", 20000m);

        // Act
        var closedAuction = _auctionService.CloseAuction("sedan-001");

        // Assert
        closedAuction.IsActive.Should().BeFalse();
        closedAuction.CurrentHighestBid.Should().Be(20000m);
        closedAuction.HighestBidder.Should().Be("John");
    }

    [Fact]
    public void CloseAuction_WithNonExistentVehicle_ShouldThrowVehicleNotFoundException()
    {
        // Act & Assert
        var ex = Assert.Throws<VehicleNotFoundException>(() => _auctionService.CloseAuction("non-existent"));
        ex.Message.Should().Contain("not found");
    }

    [Fact]
    public void CloseAuction_WhenNoActiveAuction_ShouldThrowAuctionNotActiveException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);

        // Act & Assert
        var ex = Assert.Throws<AuctionNotActiveException>(() => _auctionService.CloseAuction("sedan-001"));
        ex.Message.Should().Contain("No active auction");
    }

    [Fact]
    public void PlaceBid_AfterAuctionClosed_ShouldThrowAuctionNotActiveException()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");
        _auctionService.PlaceBid("sedan-001", "John", 20000m);
        _auctionService.CloseAuction("sedan-001");

        // Act & Assert
        var ex = Assert.Throws<AuctionNotActiveException>(() => _auctionService.PlaceBid("sedan-001", "Jane", 25000m));
        ex.Message.Should().Contain("No active auction");
    }

    [Fact]
    public void StartAuction_AfterAuctionClosed_ShouldSucceed()
    {
        // Arrange
        var vehicle = new Sedan("sedan-001", "Toyota", "Camry", 2023, 15000m, 4);
        _auctionService.AddVehicle(vehicle);
        _auctionService.StartAuction("sedan-001");
        _auctionService.CloseAuction("sedan-001");

        // Act & Assert - should not throw
        _auctionService.StartAuction("sedan-001");

        var auction = _auctionRepository.GetByVehicleId("sedan-001");
        auction!.IsActive.Should().BeTrue();
    }

    #endregion
}
