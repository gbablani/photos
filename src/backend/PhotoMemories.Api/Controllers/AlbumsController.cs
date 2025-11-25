using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoMemories.Api.Data;
using PhotoMemories.Api.DTOs;
using PhotoMemories.Api.Models;

namespace PhotoMemories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlbumsController : ControllerBase
{
    private readonly PhotoMemoriesDbContext _dbContext;

    public AlbumsController(PhotoMemoriesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Get all albums for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbums()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var albums = await _dbContext.Albums
            .Where(a => a.UserId == userId.Value)
            .Select(a => new AlbumDto(
                a.Id,
                a.Name,
                a.Description,
                a.CoverPhotoUrl,
                a.AlbumPhotos.Count,
                a.CreatedAt
            ))
            .ToListAsync();

        return Ok(albums);
    }

    /// <summary>
    /// Get a specific album with its photos
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AlbumWithPhotosDto>> GetAlbum(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var album = await _dbContext.Albums
            .Include(a => a.AlbumPhotos)
            .ThenInclude(ap => ap.Photo)
            .Where(a => a.UserId == userId.Value && a.Id == id)
            .FirstOrDefaultAsync();

        if (album == null)
            return NotFound();

        var photos = album.AlbumPhotos
            .OrderBy(ap => ap.SortOrder)
            .Select(ap => new PhotoDto(
                ap.Photo.Id,
                ap.Photo.OriginalFileName,
                ap.Photo.BlobUrl,
                ap.Photo.ThumbnailUrl,
                ap.Photo.FileSize,
                ap.Photo.Width,
                ap.Photo.Height,
                ap.Photo.DateTaken,
                ap.Photo.Location,
                ap.Photo.Description,
                ap.Photo.Tags,
                ap.Photo.IsBlackAndWhite,
                ap.Photo.Source,
                ap.Photo.IsEnhanced,
                ap.Photo.EnhancementType,
                ap.Photo.CreatedAt
            ));

        return Ok(new AlbumWithPhotosDto(
            album.Id,
            album.Name,
            album.Description,
            album.CoverPhotoUrl,
            photos,
            album.CreatedAt
        ));
    }

    /// <summary>
    /// Create a new album
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AlbumDto>> CreateAlbum([FromBody] CreateAlbumDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var album = new Album
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Albums.Add(album);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAlbum), new { id = album.Id }, 
            new AlbumDto(album.Id, album.Name, album.Description, album.CoverPhotoUrl, 0, album.CreatedAt));
    }

    /// <summary>
    /// Update album details
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<ActionResult<AlbumDto>> UpdateAlbum(Guid id, [FromBody] CreateAlbumDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var album = await _dbContext.Albums
            .Where(a => a.UserId == userId.Value && a.Id == id)
            .FirstOrDefaultAsync();

        if (album == null)
            return NotFound();

        album.Name = request.Name;
        album.Description = request.Description;
        album.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var photoCount = await _dbContext.AlbumPhotos.CountAsync(ap => ap.AlbumId == id);
        return Ok(new AlbumDto(album.Id, album.Name, album.Description, album.CoverPhotoUrl, photoCount, album.CreatedAt));
    }

    /// <summary>
    /// Delete an album
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlbum(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var album = await _dbContext.Albums
            .Where(a => a.UserId == userId.Value && a.Id == id)
            .FirstOrDefaultAsync();

        if (album == null)
            return NotFound();

        _dbContext.Albums.Remove(album);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Add photos to an album
    /// </summary>
    [HttpPost("{id}/photos")]
    public async Task<IActionResult> AddPhotosToAlbum(Guid id, [FromBody] AddPhotosToAlbumRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var album = await _dbContext.Albums
            .Where(a => a.UserId == userId.Value && a.Id == id)
            .FirstOrDefaultAsync();

        if (album == null)
            return NotFound("Album not found");

        // Verify photos belong to user
        var photos = await _dbContext.Photos
            .Where(p => p.UserId == userId.Value && request.PhotoIds.Contains(p.Id))
            .ToListAsync();

        if (photos.Count != request.PhotoIds.Count)
            return BadRequest("Some photos were not found");

        var maxSortOrder = await _dbContext.AlbumPhotos
            .Where(ap => ap.AlbumId == id)
            .Select(ap => (int?)ap.SortOrder)
            .MaxAsync() ?? 0;

        foreach (var photoId in request.PhotoIds)
        {
            var exists = await _dbContext.AlbumPhotos
                .AnyAsync(ap => ap.AlbumId == id && ap.PhotoId == photoId);

            if (!exists)
            {
                maxSortOrder++;
                _dbContext.AlbumPhotos.Add(new AlbumPhoto
                {
                    AlbumId = id,
                    PhotoId = photoId,
                    SortOrder = maxSortOrder,
                    AddedAt = DateTime.UtcNow
                });
            }
        }

        // Set cover photo if not set
        if (string.IsNullOrEmpty(album.CoverPhotoUrl) && photos.Count > 0)
        {
            album.CoverPhotoUrl = photos[0].ThumbnailUrl ?? photos[0].BlobUrl;
        }

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Remove a photo from an album
    /// </summary>
    [HttpDelete("{id}/photos/{photoId}")]
    public async Task<IActionResult> RemovePhotoFromAlbum(Guid id, Guid photoId)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var album = await _dbContext.Albums
            .Where(a => a.UserId == userId.Value && a.Id == id)
            .FirstOrDefaultAsync();

        if (album == null)
            return NotFound("Album not found");

        var albumPhoto = await _dbContext.AlbumPhotos
            .Where(ap => ap.AlbumId == id && ap.PhotoId == photoId)
            .FirstOrDefaultAsync();

        if (albumPhoto == null)
            return NotFound("Photo not in album");

        _dbContext.AlbumPhotos.Remove(albumPhoto);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
}

public record AddPhotosToAlbumRequest(List<Guid> PhotoIds);
