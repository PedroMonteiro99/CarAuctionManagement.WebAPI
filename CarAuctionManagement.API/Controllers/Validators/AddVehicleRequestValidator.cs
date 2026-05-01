using FluentValidation.Results;

namespace CarAuctionManagementAPI.Controllers.Validators;

using Requests;
using FluentValidation;

/// <summary>
/// Validator for the AddVehicleRequestDto.
/// </summary>
public class AddVehicleRequestValidator : AbstractValidator<AddVehicleRequest>, IAddVehicleRequestValidator
{
    /// <summary>
    /// Initializes a new instance of the AddVehicleRequestValidator class.
    /// </summary>
    public AddVehicleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Vehicle ID cannot be empty.")
            .NotNull().WithMessage("Vehicle ID is required.")
            .MaximumLength(100).WithMessage("Vehicle ID cannot exceed 100 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Vehicle Type cannot be empty.")
            .NotNull().WithMessage("Vehicle Type is required.")
            .Must(x => new[] { "sedan", "hatchback", "suv", "truck" }.Contains(x.ToLowerInvariant()))
            .WithMessage("Vehicle Type must be one of: Sedan, Hatchback, SUV, Truck.");

        RuleFor(x => x.Manufacturer)
            .NotEmpty().WithMessage("Manufacturer cannot be empty.")
            .NotNull().WithMessage("Manufacturer is required.")
            .MaximumLength(100).WithMessage("Manufacturer cannot exceed 100 characters.");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model cannot be empty.")
            .NotNull().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters.");

        RuleFor(x => x.Year)
            .GreaterThan(1885).WithMessage("Vehicle year must be greater than 1885.")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Vehicle year cannot be in the future.");

        RuleFor(x => x.StartingBid)
            .GreaterThan(0).WithMessage("Starting bid must be greater than 0.");

        RuleFor(x => x.NumberOfDoors)
            .GreaterThan(0).WithMessage("Number of doors must be greater than 0.")
            .When(x => x.Type.ToLowerInvariant() is "sedan" or "hatchback")
            .WithMessage("Number of doors is required for Sedan or Hatchback.");

        RuleFor(x => x.NumberOfSeats)
            .GreaterThan(0).WithMessage("Number of seats must be greater than 0.")
            .When(x => x.Type.ToLowerInvariant() == "suv")
            .WithMessage("Number of seats is required for SUV.");

        RuleFor(x => x.LoadCapacity)
            .GreaterThan(0).WithMessage("Load capacity must be greater than 0.")
            .When(x => x.Type.ToLowerInvariant() == "truck")
            .WithMessage("Load capacity is required for Truck.");
    }

    public ValidationResult ValidateVehicleDetails(AddVehicleRequest request)
    {
        var result = this.Validate(request);
        return result;
    }
}