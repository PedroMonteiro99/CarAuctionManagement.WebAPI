using CarAuctionManagementAPI.Controllers.Validators;

namespace CarAuctionManagementAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarAuctionManagement.Application.Services;
using CarAuctionManagement.Domain.Entities;
using CarAuctionManagement.Domain.Exceptions;
using Requests;
using Responses;
using System.Security.Claims;

/// <summary>
/// REST API controller for managing auction operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuctionsController : ControllerBase
{
    private readonly AuctionService _auctionService;
    private readonly IAddVehicleRequestValidator _addVehicleRequestValidator;
    private readonly IPlaceBidRequestValidator _placeBidRequestValidator;

    /// <summary>
    /// Initializes a new instance of the AuctionsController class.
    /// </summary>
    /// <param name="auctionService">The auction service for handling business logic.</param>
    public AuctionsController(AuctionService auctionService, IPlaceBidRequestValidator placeBidRequestValidator, IAddVehicleRequestValidator addVehicleRequestValidator)
    {
        _auctionService = auctionService ?? throw new ArgumentNullException(nameof(auctionService));
        _placeBidRequestValidator = placeBidRequestValidator ?? throw new ArgumentNullException(nameof(placeBidRequestValidator));
        _addVehicleRequestValidator = addVehicleRequestValidator ?? throw new ArgumentNullException(nameof(addVehicleRequestValidator));
    }

    /// <summary>
    /// Adds a new vehicle to the auction system.
    /// </summary>
    /// <param name="request">The vehicle to add.</param>
    /// <returns>A 200 OK response if the vehicle is successfully added.</returns>
    [HttpPost("vehicles")]
    public IActionResult AddVehicle([FromBody] AddVehicleRequest request)
    {
        if (request == null)
        {
            return BadRequest("Vehicle request cannot be null.");
        }

        try
        {
            var result = _addVehicleRequestValidator.ValidateVehicleDetails(request);

            if (!result.IsValid)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                throw new InvalidVehicleException($"Vehicle request is invalid: {errors}");
            }

            var vehicle = CreateVehicleFromRequest(request);
            _auctionService.AddVehicle(vehicle);
            return Ok(new { message = "Vehicle added successfully.", vehicleId = request.Id });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Searches for vehicles based on optional criteria.
    /// </summary>
    /// <param name="request">The search criteria.</param>
    /// <returns>A 200 OK response with matching vehicles.</returns>
    [HttpGet("vehicles/search")]
    public IActionResult SearchVehicles([FromQuery] SearchVehiclesRequest request)
    {
        try
        {
            var vehicles = _auctionService.SearchVehicles(
                request?.Type,
                request?.Manufacturer,
                request?.Model,
                request?.Year
            );

            var response = vehicles.Select(v => new VehicleResponse
            {
                Id = v.Id,
                Type = v.GetType().Name,
                Manufacturer = v.Manufacturer,
                Model = v.Model,
                Year = v.Year,
                StartingBid = v.StartingBid
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Searches for vehicles with pagination.
    /// </summary>
    /// <param name="type">The vehicle type (optional).</param>
    /// <param name="manufacturer">The manufacturer (optional).</param>
    /// <param name="model">The model (optional).</param>
    /// <param name="year">The year (optional).</param>
    /// <param name="pageNumber">The page number (default 1).</param>
    /// <param name="pageSize">The page size (default 10, max 100).</param>
    /// <returns>A paginated list of matching vehicles.</returns>
    [HttpGet("vehicles/search-paged")]
    public IActionResult SearchVehiclesPaged([FromQuery] string? type, [FromQuery] string? manufacturer,
        [FromQuery] string? model, [FromQuery] int? year, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var pagedResult = _auctionService.SearchVehiclesPaged(type, manufacturer, model, year, pageNumber, pageSize);

            var response = pagedResult.Items.Select(v => new VehicleResponse
            {
                Id = v.Id,
                Type = v.GetType().Name,
                Manufacturer = v.Manufacturer,
                Model = v.Model,
                Year = v.Year,
                StartingBid = v.StartingBid
            });

            return Ok(new
            {
                data = response,
                pagination = new
                {
                    pagedResult.PageNumber,
                    pagedResult.PageSize,
                    pagedResult.TotalCount,
                    pagedResult.TotalPages,
                    pagedResult.HasNextPage,
                    pagedResult.HasPreviousPage
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Starts a new auction for a vehicle.
    /// </summary>
    /// <param name="vehicleId">The ID of the vehicle to auction.</param>
    /// <returns>A 200 OK response if the auction is successfully started.</returns>
    [HttpPost("start")]
    public IActionResult StartAuction([FromQuery] string vehicleId)
    {
        if (string.IsNullOrWhiteSpace(vehicleId))
        {
            return BadRequest("Vehicle ID is required.");
        }

        try
        {
            _auctionService.StartAuction(vehicleId);
            return Ok(new { message = "Auction started successfully.", vehicleId });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Places a bid on an active auction.
    /// </summary>
    /// <param name="request">The bid details.</param>
    /// <returns>A 200 OK response if the bid is successfully placed.</returns>
    [HttpPost("bid")]
    public IActionResult PlaceBid([FromBody] PlaceBidRequest request)
    {
        var result = _placeBidRequestValidator.ValidateBidDetails(request);

        if (!result.IsValid)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
            throw new InvalidBidException($"Bid request is invalid: {errors}");
        }

        var bidder = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User?.FindFirst("sub")?.Value
                     ?? User?.Identity?.Name
                     ?? User?.FindFirst(ClaimTypes.Name)?.Value
                     ?? User?.FindFirst("preferred_username")?.Value;

        if (string.IsNullOrWhiteSpace(bidder))
        {
            return Unauthorized(new { error = "Unable to determine bidder from authentication." });
        }

        try
        {
            _auctionService.PlaceBid(request.VehicleId, bidder, request.Amount);
            return Ok(new { message = "Bid placed successfully.", vehicleId = request.VehicleId, bidder, amount = request.Amount });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Closes an active auction.
    /// </summary>
    /// <param name="vehicleId">The ID of the vehicle being auctioned.</param>
    /// <returns>A 200 OK response with the final auction state.</returns>
    [HttpPost("close")]
    public IActionResult CloseAuction([FromQuery] string vehicleId)
    {
        if (string.IsNullOrWhiteSpace(vehicleId))
        {
            return BadRequest("Vehicle ID is required.");
        }

        try
        {
            var auction = _auctionService.CloseAuction(vehicleId);
            var response = new AuctionResponse
            {
                VehicleId = auction.VehicleId,
                CurrentHighestBid = auction.CurrentHighestBid,
                HighestBidder = auction.HighestBidder,
                IsActive = auction.IsActive
            };
            return Ok(new { message = "Auction closed successfully.", auction = response });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a vehicle instance based on the type specified in the request.
    /// </summary>
    private static Vehicle CreateVehicleFromRequest(AddVehicleRequest request)
    {
        return request.Type.ToLowerInvariant() switch
        {
            "sedan" => new Sedan(request.Id, request.Manufacturer, request.Model, request.Year, request.StartingBid, request.NumberOfDoors ?? 4),
            "hatchback" => new Hatchback(request.Id, request.Manufacturer, request.Model, request.Year, request.StartingBid, request.NumberOfDoors ?? 5),
            "suv" => new Suv(request.Id, request.Manufacturer, request.Model, request.Year, request.StartingBid, request.NumberOfSeats ?? 7),
            "truck" => new Truck(request.Id, request.Manufacturer, request.Model, request.Year, request.StartingBid, request.LoadCapacity ?? 5),
            _ => throw new InvalidOperationException($"Unknown vehicle type: {request.Type}")
        };
    }
}