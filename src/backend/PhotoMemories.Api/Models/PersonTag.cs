namespace PhotoMemories.Api.Models;

public class PersonTag
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PhotoId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string? PersonId { get; set; }  // For face recognition grouping
    
    // Bounding box for the face in the photo (normalized 0-1)
    public float? FaceX { get; set; }
    public float? FaceY { get; set; }
    public float? FaceWidth { get; set; }
    public float? FaceHeight { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User User { get; set; } = null!;
    public virtual Photo Photo { get; set; } = null!;
}
