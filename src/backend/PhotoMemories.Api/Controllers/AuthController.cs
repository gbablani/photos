using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoMemories.Api.DTOs;
using PhotoMemories.Api.Services;

namespace PhotoMemories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticate with an external provider (Google, Microsoft, Facebook)
    /// </summary>
    [HttpPost("external")]
    public async Task<ActionResult<AuthResponseDto>> ExternalAuth([FromBody] ExternalAuthDto request)
    {
        try
        {
            // TODO: Validate the external token with the provider
            // For now, we'll simulate token validation
            // In production, use Microsoft.Identity.Web or similar to validate tokens

            // Simulated user info from external provider
            var externalId = Guid.NewGuid().ToString(); // Would come from token validation
            var email = "user@example.com"; // Would come from token validation
            var displayName = "User Name"; // Would come from token validation
            string? profilePicture = null;

            var user = await _userService.GetOrCreateUserAsync(
                request.Provider, 
                externalId, 
                email, 
                displayName, 
                profilePicture
            );

            // Get the actual user entity to generate tokens
            var userEntity = await GetUserEntityAsync(user.Id);
            if (userEntity == null)
                return BadRequest("Failed to create user");

            var authResponse = await _userService.GenerateAuthTokensAsync(userEntity);
            return Ok(authResponse);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var user = await _userService.GetUserAsync(userId.Value);
            return Ok(user);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    [HttpPatch("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UserProfileUpdateDto update)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var user = await _userService.UpdateUserAsync(userId.Value, update);
            return Ok(user);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }

    private async Task<Models.User?> GetUserEntityAsync(Guid userId)
    {
        // This is a workaround - in production, the UserService should handle this
        using var scope = HttpContext.RequestServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Data.PhotoMemoriesDbContext>();
        return await dbContext.Users.FindAsync(userId);
    }
}
