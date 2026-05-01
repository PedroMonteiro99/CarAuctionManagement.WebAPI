namespace CarAuctionManagement.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-level exceptions.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the DomainException class.
    /// </summary>
    protected DomainException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the DomainException class.
    /// </summary>
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}
