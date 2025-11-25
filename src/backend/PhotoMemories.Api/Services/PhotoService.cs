using Microsoft.EntityFrameworkCore;
using PhotoMemories.Api.Data;
using PhotoMemories.Api.DTOs;
using PhotoMemories.Api.Models;

namespace PhotoMemories.Api.Services;

public interface IPhotoService
{
    Task<PhotoDto> GetPhotoAsync(Guid userId, Guid photoId);
    Task<PhotoListResponseDto> GetPhotosAsync(Guid userId, PhotoSearchDto search);
    Task<PhotoDto> CreatePhotoAsync(Guid userId, Photo photo);
    Task<PhotoDto> UpdatePhotoAsync(Guid userId, Guid photoId, string? description, string? tags);
    Task DeletePhotoAsync(Guid userId, Guid photoId);
    Task<IEnumerable<PhotoDto>> GetPhotosByPersonAsync(Guid userId, string personName);
    Task<IEnumerable<PhotoDto>> GetPhotosByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
}

public class PhotoService : IPhotoService
{
    private readonly PhotoMemoriesDbContext _dbContext;
    private readonly IBlobStorageService _blobStorageService;

    public PhotoService(PhotoMemoriesDbContext dbContext, IBlobStorageService blobStorageService)
    {
        _dbContext = dbContext;
        _blobStorageService = blobStorageService;
    }

    public async Task<PhotoDto> GetPhotoAsync(Guid userId, Guid photoId)
    {
        var photo = await _dbContext.Photos
            .Where(p => p.UserId == userId && p.Id == photoId)
            .FirstOrDefaultAsync();

        if (photo == null)
            throw new KeyNotFoundException($"Photo {photoId} not found");

        return MapToDto(photo);
    }

    public async Task<PhotoListResponseDto> GetPhotosAsync(Guid userId, PhotoSearchDto search)
    {
        var query = _dbContext.Photos.Where(p => p.UserId == userId);

        // Apply search filters
        if (!string.IsNullOrEmpty(search.Query))
        {
            var queryLower = search.Query.ToLower();
            query = query.Where(p => 
                (p.Description != null && p.Description.ToLower().Contains(queryLower)) ||
                (p.Tags != null && p.Tags.ToLower().Contains(queryLower)) ||
                p.OriginalFileName.ToLower().Contains(queryLower));
        }

        if (!string.IsNullOrEmpty(search.PersonName))
        {
            query = query.Where(p => p.PersonTags.Any(t => t.PersonName == search.PersonName));
        }

        if (search.StartDate.HasValue)
        {
            query = query.Where(p => p.DateTaken >= search.StartDate.Value);
        }

        if (search.EndDate.HasValue)
        {
            query = query.Where(p => p.DateTaken <= search.EndDate.Value);
        }

        if (search.Source.HasValue)
        {
            query = query.Where(p => p.Source == search.Source.Value);
        }

        if (search.IsEnhanced.HasValue)
        {
            query = query.Where(p => p.IsEnhanced == search.IsEnhanced.Value);
        }

        var totalCount = await query.CountAsync();

        var photos = await query
            .OrderByDescending(p => p.DateTaken ?? p.CreatedAt)
            .Skip((search.Page - 1) * search.PageSize)
            .Take(search.PageSize)
            .ToListAsync();

        return new PhotoListResponseDto(
            photos.Select(MapToDto),
            totalCount,
            search.Page,
            search.PageSize
        );
    }

    public async Task<PhotoDto> CreatePhotoAsync(Guid userId, Photo photo)
    {
        photo.UserId = userId;
        photo.CreatedAt = DateTime.UtcNow;
        photo.UpdatedAt = DateTime.UtcNow;

        _dbContext.Photos.Add(photo);
        await _dbContext.SaveChangesAsync();

        return MapToDto(photo);
    }

    public async Task<PhotoDto> UpdatePhotoAsync(Guid userId, Guid photoId, string? description, string? tags)
    {
        var photo = await _dbContext.Photos
            .Where(p => p.UserId == userId && p.Id == photoId)
            .FirstOrDefaultAsync();

        if (photo == null)
            throw new KeyNotFoundException($"Photo {photoId} not found");

        if (description != null)
            photo.Description = description;

        if (tags != null)
            photo.Tags = tags;

        photo.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return MapToDto(photo);
    }

    public async Task DeletePhotoAsync(Guid userId, Guid photoId)
    {
        var photo = await _dbContext.Photos
            .Where(p => p.UserId == userId && p.Id == photoId)
            .FirstOrDefaultAsync();

        if (photo == null)
            throw new KeyNotFoundException($"Photo {photoId} not found");

        // Delete blob
        await _blobStorageService.DeleteFileAsync(photo.BlobUrl);
        if (!string.IsNullOrEmpty(photo.ThumbnailUrl))
        {
            await _blobStorageService.DeleteFileAsync(photo.ThumbnailUrl);
        }

        _dbContext.Photos.Remove(photo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<PhotoDto>> GetPhotosByPersonAsync(Guid userId, string personName)
    {
        var photos = await _dbContext.Photos
            .Where(p => p.UserId == userId && p.PersonTags.Any(t => t.PersonName == personName))
            .OrderByDescending(p => p.DateTaken ?? p.CreatedAt)
            .ToListAsync();

        return photos.Select(MapToDto);
    }

    public async Task<IEnumerable<PhotoDto>> GetPhotosByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        var photos = await _dbContext.Photos
            .Where(p => p.UserId == userId && p.DateTaken >= startDate && p.DateTaken <= endDate)
            .OrderBy(p => p.DateTaken)
            .ToListAsync();

        return photos.Select(MapToDto);
    }

    private static PhotoDto MapToDto(Photo photo)
    {
        return new PhotoDto(
            photo.Id,
            photo.OriginalFileName,
            photo.BlobUrl,
            photo.ThumbnailUrl,
            photo.FileSize,
            photo.Width,
            photo.Height,
            photo.DateTaken,
            photo.Location,
            photo.Description,
            photo.Tags,
            photo.IsBlackAndWhite,
            photo.Source,
            photo.IsEnhanced,
            photo.EnhancementType,
            photo.CreatedAt
        );
    }
}
