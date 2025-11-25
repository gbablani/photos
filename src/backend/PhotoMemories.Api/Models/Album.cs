namespace PhotoMemories.Api.Models;

public class Album
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverPhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<AlbumPhoto> AlbumPhotos { get; set; } = new List<AlbumPhoto>();
}

public class AlbumPhoto
{
    public Guid AlbumId { get; set; }
    public Guid PhotoId { get; set; }
    public int SortOrder { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Album Album { get; set; } = null!;
    public virtual Photo Photo { get; set; } = null!;
}
