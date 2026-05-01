namespace CarAuctionManagementAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using CarAuctionManagement.Application.Services;
using Requests;

/// <summary>
/// REST API controller for authentication operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;

    /// <summary>
    /// Initializes a new instance of the AuthController class.
    /// </summary>
    public AuthController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <returns>A JWT token if authentication is successful.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { error = "Login request cannot be null." });
        }

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = "Username and password are required." });
        }

        // Simple authentication (in production, validate against a user database)
        if (!IsValidUser(request.Username, request.Password))
        {
            return Unauthorized(new { error = "Invalid username or password." });
        }

        var token = _jwtTokenService.GenerateToken(request.Username, request.Username, "bidder");
        return Ok(new { token, message = "Login successful." });
    }

    /// <summary>
    /// Validates user credentials (simplified for demonstration).
    /// In production, this should check against a user database with hashed passwords.
    /// </summary>
    private static bool IsValidUser(string username, string password)
    {
        // Demo users - in production, validate against database
        var validUsers = new Dictionary<string, string>
        {
            { "admin", "admin123" },
            { "bidder1", "password123" },
            { "bidder2", "password123" }
        };

        return validUsers.ContainsKey(username) && validUsers[username] == password;
    }
}
