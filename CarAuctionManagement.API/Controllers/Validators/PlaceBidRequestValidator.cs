using FluentValidation.Results;

namespace CarAuctionManagementAPI.Controllers.Validators;

using FluentValidation;
using Requests;

/// <summary>
/// Validator for the PlaceBidRequestDto.
/// </summary>
public class PlaceBidRequestValidator : AbstractValidator<PlaceBidRequest>, IPlaceBidRequestValidator
{
    /// <summary>
    /// Initializes a new instance of the PlaceBidRequestValidator class.
    /// </summary>
    public PlaceBidRequestValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("Vehicle ID cannot be empty.")
            .NotNull().WithMessage("Vehicle ID is required.")
            .MaximumLength(100).WithMessage("Vehicle ID cannot exceed 100 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Bid amount must be greater than 0.");
    }

    public ValidationResult ValidateBidDetails(PlaceBidRequest request)
    {
        var result = this.Validate(request);
        return result;
    }
}