using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoMemories.Api.DTOs;
using PhotoMemories.Api.Models;
using PhotoMemories.Api.Services;

namespace PhotoMemories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PhotosController : ControllerBase
{
    private readonly IPhotoService _photoService;
    private readonly IBlobStorageService _blobStorageService;

    public PhotosController(IPhotoService photoService, IBlobStorageService blobStorageService)
    {
        _photoService = photoService;
        _blobStorageService = blobStorageService;
    }

    /// <summary>
    /// Get all photos for the current user with optional search filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PhotoListResponseDto>> GetPhotos([FromQuery] PhotoSearchDto search)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _photoService.GetPhotosAsync(userId.Value, search);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific photo
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PhotoDto>> GetPhoto(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var photo = await _photoService.GetPhotoAsync(userId.Value, id);
            return Ok(photo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Upload a new photo
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PhotoDto>> UploadPhoto([FromForm] IFormFile file, [FromForm] PhotoUploadDto metadata)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "image/tiff" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest("Invalid file type. Allowed: JPEG, PNG, GIF, WebP, TIFF");

        // Upload to blob storage
        using var stream = file.OpenReadStream();
        var blobUrl = await _blobStorageService.UploadPhotoAsync(userId.Value, stream, file.FileName, file.ContentType);

        // Create photo record
        var photo = new Photo
        {
            Id = Guid.NewGuid(),
            OriginalFileName = metadata.FileName ?? file.FileName,
            BlobUrl = blobUrl,
            FileSize = file.Length,
            ContentType = file.ContentType,
            DateTaken = metadata.DateTaken,
            Description = metadata.Description,
            Tags = metadata.Tags,
            Source = PhotoSource.Upload
        };

        var result = await _photoService.CreatePhotoAsync(userId.Value, photo);
        return CreatedAtAction(nameof(GetPhoto), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a pre-signed upload URL for direct client upload
    /// </summary>
    [HttpPost("upload-url")]
    public async Task<ActionResult<object>> GetUploadUrl([FromBody] PhotoUploadDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var uploadUrl = await _blobStorageService.GenerateUploadSasUrlAsync(
            userId.Value, 
            request.FileName, 
            request.ContentType, 
            false
        );

        return Ok(new { uploadUrl });
    }

    /// <summary>
    /// Update photo metadata
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<ActionResult<PhotoDto>> UpdatePhoto(Guid id, [FromBody] PhotoUpdateRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var photo = await _photoService.UpdatePhotoAsync(userId.Value, id, request.Description, request.Tags);
            return Ok(photo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a photo
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            await _photoService.DeletePhotoAsync(userId.Value, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Search photos by person name
    /// </summary>
    [HttpGet("by-person/{personName}")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosByPerson(string personName)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var photos = await _photoService.GetPhotosByPersonAsync(userId.Value, personName);
        return Ok(photos);
    }

    /// <summary>
    /// Get photos by date range (timeline view)
    /// </summary>
    [HttpGet("by-date")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var photos = await _photoService.GetPhotosByDateRangeAsync(userId.Value, startDate, endDate);
        return Ok(photos);
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
}

public record PhotoUpdateRequest(string? Description, string? Tags);
