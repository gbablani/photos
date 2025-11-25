using PhotoMemories.Api.Models;

namespace PhotoMemories.Api.DTOs;

// User DTOs
public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string? ProfilePictureUrl,
    SubscriptionTier SubscriptionTier,
    int FreeEnhancementsRemaining,
    int EnhancementCredits,
    DateTime? SubscriptionExpiresAt,
    bool GooglePhotosConnected,
    bool OneDriveConnected,
    bool AutoSyncEnabled
);

public record UserProfileUpdateDto(
    string? DisplayName,
    bool? AutoSyncEnabled
);

// Authentication DTOs
public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    UserDto User,
    DateTime ExpiresAt
);

public record ExternalAuthDto(
    AuthProvider Provider,
    string AccessToken,
    string? RefreshToken
);

// Photo DTOs
public record PhotoDto(
    Guid Id,
    string OriginalFileName,
    string BlobUrl,
    string? ThumbnailUrl,
    long FileSize,
    int Width,
    int Height,
    DateTime? DateTaken,
    string? Location,
    string? Description,
    string? Tags,
    bool IsBlackAndWhite,
    PhotoSource Source,
    bool IsEnhanced,
    EnhancementType? EnhancementType,
    DateTime CreatedAt
);

public record PhotoUploadDto(
    string FileName,
    string ContentType,
    DateTime? DateTaken,
    string? Description,
    string? Tags
);

public record PhotoSearchDto(
    string? Query,
    string? PersonName,
    DateTime? StartDate,
    DateTime? EndDate,
    PhotoSource? Source,
    bool? IsEnhanced,
    int Page = 1,
    int PageSize = 20
);

public record PhotoListResponseDto(
    IEnumerable<PhotoDto> Photos,
    int TotalCount,
    int Page,
    int PageSize
);

// Video DTOs
public record VideoDto(
    Guid Id,
    string OriginalFileName,
    string BlobUrl,
    string? ThumbnailUrl,
    long FileSize,
    int Width,
    int Height,
    double DurationSeconds,
    DateTime? DateTaken,
    string? Description,
    VideoSource Source,
    bool IsGenerated,
    VideoGenerationType? GenerationType,
    bool IsEnhanced,
    DateTime CreatedAt
);

// Album DTOs
public record AlbumDto(
    Guid Id,
    string Name,
    string? Description,
    string? CoverPhotoUrl,
    int PhotoCount,
    DateTime CreatedAt
);

public record CreateAlbumDto(
    string Name,
    string? Description
);

public record AlbumWithPhotosDto(
    Guid Id,
    string Name,
    string? Description,
    string? CoverPhotoUrl,
    IEnumerable<PhotoDto> Photos,
    DateTime CreatedAt
);

// Person Tag DTOs
public record PersonTagDto(
    Guid Id,
    string PersonName,
    Guid PhotoId
);

public record CreatePersonTagDto(
    Guid PhotoId,
    string PersonName,
    float? FaceX,
    float? FaceY,
    float? FaceWidth,
    float? FaceHeight
);

// Enhancement DTOs
public record EnhancementJobDto(
    Guid Id,
    JobType JobType,
    JobStatus Status,
    int ProgressPercent,
    string? StatusMessage,
    Guid? SourcePhotoId,
    Guid? SourceVideoId,
    Guid? ResultPhotoId,
    Guid? ResultVideoId,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime? CompletedAt
);

public record CreateEnhancementJobDto(
    JobType JobType,
    Guid? SourcePhotoId,
    Guid? SourceVideoId,
    List<Guid>? AdditionalPhotoIds,  // For montage
    EnhancementOptions? Options
);

public record EnhancementOptions(
    AnimationStyle? AnimationStyle,
    bool? AddMusic,
    int? DurationSeconds,
    Guid? PersonPhotoId  // For "Add Person to Video"
);

public enum AnimationStyle
{
    KenBurns,
    Parallax,
    SlowZoom,
    CrossFade
}

// Subscription DTOs
public record SubscriptionStatusDto(
    SubscriptionTier Tier,
    int FreeEnhancementsRemaining,
    int EnhancementCredits,
    DateTime? ExpiresAt,
    bool CanEnhance
);

public record PurchaseCreditsDto(
    int CreditPackage  // 10, 25, 50, etc.
);

public record SubscribeDto(
    SubscriptionTier Tier  // PayAsYouGo or Premium
);

// External Integration DTOs
public record ConnectServiceDto(
    string AuthorizationCode,
    string? RedirectUri
);

public record ExternalPhotosDto(
    IEnumerable<ExternalPhotoDto> Photos,
    string? NextPageToken
);

public record ExternalPhotoDto(
    string ExternalId,
    string FileName,
    string? ThumbnailUrl,
    DateTime? DateTaken,
    long? FileSize
);

public record ImportPhotosDto(
    List<string> ExternalIds
);
