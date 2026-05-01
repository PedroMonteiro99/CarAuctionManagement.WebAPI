using FluentValidation.Results;

namespace CarAuctionManagementAPI.Controllers.Validators;

using Requests;

/// <summary>
/// Interface for validating place bid requests.
/// </summary>
public interface IPlaceBidRequestValidator
{
    /// <summary>
    /// Validates the place bid request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <returns>The FluentValidation Validation Result</returns>
    ValidationResult ValidateBidDetails(PlaceBidRequest request);
}