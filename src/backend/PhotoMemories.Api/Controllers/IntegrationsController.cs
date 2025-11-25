using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoMemories.Api.Data;
using PhotoMemories.Api.DTOs;

namespace PhotoMemories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IntegrationsController : ControllerBase
{
    private readonly PhotoMemoriesDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public IntegrationsController(PhotoMemoriesDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    /// <summary>
    /// Connect to Google Photos
    /// </summary>
    [HttpPost("google-photos/connect")]
    public async Task<ActionResult> ConnectGooglePhotos([FromBody] ConnectServiceDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        // TODO: Exchange authorization code for access/refresh tokens using Google OAuth
        // Store tokens securely
        // For now, just mark as connected

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null)
            return NotFound("User not found");

        user.GooglePhotosConnected = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Google Photos connected successfully" });
    }

    /// <summary>
    /// Disconnect from Google Photos
    /// </summary>
    [HttpPost("google-photos/disconnect")]
    public async Task<ActionResult> DisconnectGooglePhotos()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null)
            return NotFound("User not found");

        // TODO: Revoke tokens with Google

        user.GooglePhotosConnected = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Google Photos disconnected" });
    }

    /// <summary>
    /// Browse Google Photos library
    /// </summary>
    [HttpGet("google-photos/browse")]
    public async Task<ActionResult<ExternalPhotosDto>> BrowseGooglePhotos(
        [FromQuery] string? pageToken, 
        [FromQuery] int pageSize = 50)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null || !user.GooglePhotosConnected)
            return BadRequest("Google Photos not connected");

        // TODO: Call Google Photos API to list photos
        // Return mock data for now
        var mockPhotos = Enumerable.Range(1, pageSize).Select(i => new ExternalPhotoDto(
            $"google-photo-{i}",
            $"photo_{i}.jpg",
            $"https://lh3.googleusercontent.com/sample-thumbnail-{i}",
            DateTime.UtcNow.AddDays(-i),
            (long)(1024 * 1024 * Random.Shared.NextDouble() * 10)
        ));

        return Ok(new ExternalPhotosDto(mockPhotos, null));
    }

    /// <summary>
    /// Import photos from Google Photos
    /// </summary>
    [HttpPost("google-photos/import")]
    public async Task<ActionResult> ImportFromGooglePhotos([FromBody] ImportPhotosDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null || !user.GooglePhotosConnected)
            return BadRequest("Google Photos not connected");

        // TODO: Fetch photos from Google Photos API and save to blob storage
        // Create Photo records in database

        return Ok(new { 
            message = $"Import job created for {request.ExternalIds.Count} photos",
            count = request.ExternalIds.Count
        });
    }

    /// <summary>
    /// Connect to OneDrive
    /// </summary>
    [HttpPost("onedrive/connect")]
    public async Task<ActionResult> ConnectOneDrive([FromBody] ConnectServiceDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        // TODO: Exchange authorization code for access/refresh tokens using Microsoft Graph
        // Store tokens securely

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null)
            return NotFound("User not found");

        user.OneDriveConnected = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "OneDrive connected successfully" });
    }

    /// <summary>
    /// Disconnect from OneDrive
    /// </summary>
    [HttpPost("onedrive/disconnect")]
    public async Task<ActionResult> DisconnectOneDrive()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null)
            return NotFound("User not found");

        // TODO: Revoke tokens

        user.OneDriveConnected = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "OneDrive disconnected" });
    }

    /// <summary>
    /// Browse OneDrive photos
    /// </summary>
    [HttpGet("onedrive/browse")]
    public async Task<ActionResult<ExternalPhotosDto>> BrowseOneDrive(
        [FromQuery] string? path,
        [FromQuery] string? pageToken, 
        [FromQuery] int pageSize = 50)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null || !user.OneDriveConnected)
            return BadRequest("OneDrive not connected");

        // TODO: Call Microsoft Graph API to list photos
        // Return mock data for now
        var mockPhotos = Enumerable.Range(1, pageSize).Select(i => new ExternalPhotoDto(
            $"onedrive-photo-{i}",
            $"IMG_{i:D4}.jpg",
            null,
            DateTime.UtcNow.AddDays(-i * 2),
            (long)(1024 * 1024 * Random.Shared.NextDouble() * 15)
        ));

        return Ok(new ExternalPhotosDto(mockPhotos, null));
    }

    /// <summary>
    /// Import photos from OneDrive
    /// </summary>
    [HttpPost("onedrive/import")]
    public async Task<ActionResult> ImportFromOneDrive([FromBody] ImportPhotosDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null || !user.OneDriveConnected)
            return BadRequest("OneDrive not connected");

        // TODO: Fetch photos from OneDrive and save to blob storage
        // Create Photo records in database

        return Ok(new { 
            message = $"Import job created for {request.ExternalIds.Count} photos",
            count = request.ExternalIds.Count
        });
    }

    /// <summary>
    /// Export enhanced photo back to Google Photos
    /// </summary>
    [HttpPost("google-photos/export/{photoId}")]
    public async Task<ActionResult> ExportToGooglePhotos(Guid photoId)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null || !user.GooglePhotosConnected)
            return BadRequest("Google Photos not connected");

        var photo = await _dbContext.Photos
            .Where(p => p.UserId == userId.Value && p.Id == photoId)
            .FirstOrDefaultAsync();

        if (photo == null)
            return NotFound("Photo not found");

        // TODO: Upload photo to Google Photos using their upload API

        return Ok(new { message = "Photo exported to Google Photos" });
    }

    /// <summary>
    /// Export enhanced photo back to OneDrive
    /// </summary>
    [HttpPost("onedrive/export/{photoId}")]
    public async Task<ActionResult> ExportToOneDrive(Guid photoId, [FromQuery] string? targetPath)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var user = await _dbContext.Users.FindAsync(userId.Value);
        if (user == null || !user.OneDriveConnected)
            return BadRequest("OneDrive not connected");

        var photo = await _dbContext.Photos
            .Where(p => p.UserId == userId.Value && p.Id == photoId)
            .FirstOrDefaultAsync();

        if (photo == null)
            return NotFound("Photo not found");

        // TODO: Upload photo to OneDrive using Microsoft Graph API

        return Ok(new { message = "Photo exported to OneDrive" });
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
}
