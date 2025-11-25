namespace PhotoMemories.Api.Models;

public class Photo
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    
    // Metadata
    public DateTime? DateTaken { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public bool IsBlackAndWhite { get; set; }
    
    // Source info
    public PhotoSource Source { get; set; } = PhotoSource.Upload;
    public string? ExternalId { get; set; }
    
    // Enhancement status
    public bool IsEnhanced { get; set; }
    public Guid? OriginalPhotoId { get; set; }
    public EnhancementType? EnhancementType { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Photo? OriginalPhoto { get; set; }
    public virtual ICollection<Photo> EnhancedVersions { get; set; } = new List<Photo>();
    public virtual ICollection<AlbumPhoto> AlbumPhotos { get; set; } = new List<AlbumPhoto>();
    public virtual ICollection<PersonTag> PersonTags { get; set; } = new List<PersonTag>();
}

public enum PhotoSource
{
    Upload,
    GooglePhotos,
    OneDrive,
    Dropbox,
    ICloud
}

public enum EnhancementType
{
    Colorize,
    Restore,
    Upscale,
    LightingCorrection
}
