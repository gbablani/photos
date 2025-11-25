namespace PhotoMemories.Api.Models;

public class Video
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
    public double DurationSeconds { get; set; }
    
    // Metadata
    public DateTime? DateTaken { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    
    // Source info
    public VideoSource Source { get; set; } = VideoSource.Upload;
    public string? ExternalId { get; set; }
    
    // For generated videos (from photo animations)
    public bool IsGenerated { get; set; }
    public Guid? SourcePhotoId { get; set; }
    public VideoGenerationType? GenerationType { get; set; }
    
    // Enhancement status
    public bool IsEnhanced { get; set; }
    public Guid? OriginalVideoId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Photo? SourcePhoto { get; set; }
    public virtual Video? OriginalVideo { get; set; }
    public virtual ICollection<Video> EnhancedVersions { get; set; } = new List<Video>();
}

public enum VideoSource
{
    Upload,
    GooglePhotos,
    OneDrive,
    Generated
}

public enum VideoGenerationType
{
    SinglePhotoAnimation,    // Ken Burns effect, parallax motion
    MultiPhotoMontage,       // Slideshow with transitions
    PersonAdded,             // Person added to existing video
    Extended                 // Video extended with AI
}
