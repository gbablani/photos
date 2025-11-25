namespace PhotoMemories.Api.Models;

public class EnhancementJob
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? SourcePhotoId { get; set; }
    public Guid? SourceVideoId { get; set; }
    public JobType JobType { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public int CreditsUsed { get; set; } = 1;
    
    // Job parameters (JSON)
    public string? Parameters { get; set; }
    
    // Results
    public Guid? ResultPhotoId { get; set; }
    public Guid? ResultVideoId { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Progress tracking
    public int ProgressPercent { get; set; }
    public string? StatusMessage { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Photo? SourcePhoto { get; set; }
    public virtual Video? SourceVideo { get; set; }
    public virtual Photo? ResultPhoto { get; set; }
    public virtual Video? ResultVideo { get; set; }
}

public enum JobType
{
    // Photo enhancements
    Colorize,
    RestoreQuality,
    Upscale,
    LightingCorrection,
    
    // Photo to video
    SinglePhotoAnimation,
    MultiPhotoMontage,
    
    // Video enhancements
    AddPersonToVideo,
    ExtendVideo,
    VideoUpscale
}

public enum JobStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled
}
