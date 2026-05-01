using FluentValidation.Results;

namespace CarAuctionManagementAPI.Controllers.Validators;

using Requests;

/// <summary>
/// Interface for validating add vehicle requests.
/// </summary>
public interface IAddVehicleRequestValidator
{
    /// <summary>
    /// Validates the add vehicle request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <returns>The FluentValidation Validation Result</returns>
    ValidationResult ValidateVehicleDetails(AddVehicleRequest request);
}