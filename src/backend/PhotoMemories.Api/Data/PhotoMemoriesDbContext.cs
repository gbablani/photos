using Microsoft.EntityFrameworkCore;
using PhotoMemories.Api.Models;

namespace PhotoMemories.Api.Data;

public class PhotoMemoriesDbContext : DbContext
{
    public PhotoMemoriesDbContext(DbContextOptions<PhotoMemoriesDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Video> Videos => Set<Video>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<AlbumPhoto> AlbumPhotos => Set<AlbumPhoto>();
    public DbSet<PersonTag> PersonTags => Set<PersonTag>();
    public DbSet<EnhancementJob> EnhancementJobs => Set<EnhancementJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => new { e.AuthProvider, e.ExternalId }).IsUnique();
        });

        // Photo configuration
        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Photos)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.OriginalPhoto)
                .WithMany(p => p.EnhancedVersions)
                .HasForeignKey(e => e.OriginalPhotoId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.DateTaken);
        });

        // Video configuration
        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Videos)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.SourcePhoto)
                .WithMany()
                .HasForeignKey(e => e.SourcePhotoId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.OriginalVideo)
                .WithMany(v => v.EnhancedVersions)
                .HasForeignKey(e => e.OriginalVideoId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.UserId);
        });

        // Album configuration
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Albums)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AlbumPhoto (junction table)
        modelBuilder.Entity<AlbumPhoto>(entity =>
        {
            entity.HasKey(e => new { e.AlbumId, e.PhotoId });
            entity.HasOne(e => e.Album)
                .WithMany(a => a.AlbumPhotos)
                .HasForeignKey(e => e.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Photo)
                .WithMany(p => p.AlbumPhotos)
                .HasForeignKey(e => e.PhotoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PersonTag configuration
        modelBuilder.Entity<PersonTag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Photo)
                .WithMany(p => p.PersonTags)
                .HasForeignKey(e => e.PhotoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.PersonName);
        });

        // EnhancementJob configuration
        modelBuilder.Entity<EnhancementJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.EnhancementJobs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.SourcePhoto)
                .WithMany()
                .HasForeignKey(e => e.SourcePhotoId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.SourceVideo)
                .WithMany()
                .HasForeignKey(e => e.SourceVideoId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
        });
    }
}
