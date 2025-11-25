namespace PhotoMemories.Api.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public AuthProvider AuthProvider { get; set; }
    public string? ExternalId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Subscription info
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;
    public int FreeEnhancementsRemaining { get; set; } = 2;
    public int EnhancementCredits { get; set; } = 0;
    public DateTime? SubscriptionExpiresAt { get; set; }
    
    // Connected services
    public bool GooglePhotosConnected { get; set; }
    public bool OneDriveConnected { get; set; }
    public bool AutoSyncEnabled { get; set; }
    
    // Navigation properties
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
    public virtual ICollection<EnhancementJob> EnhancementJobs { get; set; } = new List<EnhancementJob>();
}

public enum AuthProvider
{
    Microsoft,
    Google,
    Facebook
}

public enum SubscriptionTier
{
    Free,
    PayAsYouGo,
    Premium
}
