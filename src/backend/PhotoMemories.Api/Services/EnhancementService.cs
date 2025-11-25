using Microsoft.EntityFrameworkCore;
using PhotoMemories.Api.Data;
using PhotoMemories.Api.DTOs;
using PhotoMemories.Api.Models;

namespace PhotoMemories.Api.Services;

public interface IEnhancementService
{
    Task<EnhancementJobDto> CreateJobAsync(Guid userId, CreateEnhancementJobDto request);
    Task<EnhancementJobDto> GetJobAsync(Guid userId, Guid jobId);
    Task<IEnumerable<EnhancementJobDto>> GetUserJobsAsync(Guid userId, JobStatus? status = null);
    Task<bool> CanUserEnhanceAsync(Guid userId);
    Task<SubscriptionStatusDto> GetSubscriptionStatusAsync(Guid userId);
    Task<bool> PurchaseCreditsAsync(Guid userId, int credits);
    Task<bool> SubscribeAsync(Guid userId, SubscriptionTier tier);
}

public class EnhancementService : IEnhancementService
{
    private readonly PhotoMemoriesDbContext _dbContext;

    public EnhancementService(PhotoMemoriesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EnhancementJobDto> CreateJobAsync(Guid userId, CreateEnhancementJobDto request)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Check if user can enhance
        if (!CanEnhance(user))
            throw new InvalidOperationException("No enhancement credits available. Please purchase credits or subscribe.");

        // Calculate credits needed
        var creditsNeeded = CalculateCreditsNeeded(request.JobType);

        // Deduct credits
        if (user.SubscriptionTier == SubscriptionTier.Premium)
        {
            // Premium users have unlimited enhancements
        }
        else if (user.FreeEnhancementsRemaining > 0)
        {
            user.FreeEnhancementsRemaining -= 1;
        }
        else if (user.EnhancementCredits >= creditsNeeded)
        {
            user.EnhancementCredits -= creditsNeeded;
        }
        else
        {
            throw new InvalidOperationException("Insufficient credits");
        }

        var job = new EnhancementJob
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            JobType = request.JobType,
            SourcePhotoId = request.SourcePhotoId,
            SourceVideoId = request.SourceVideoId,
            CreditsUsed = creditsNeeded,
            Status = JobStatus.Pending,
            Parameters = System.Text.Json.JsonSerializer.Serialize(request.Options),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.EnhancementJobs.Add(job);
        await _dbContext.SaveChangesAsync();

        // TODO: Queue job for processing (Azure Function, Background Service, etc.)
        // For now, simulate immediate processing start
        job.Status = JobStatus.Processing;
        job.StartedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return MapToDto(job);
    }

    public async Task<EnhancementJobDto> GetJobAsync(Guid userId, Guid jobId)
    {
        var job = await _dbContext.EnhancementJobs
            .Where(j => j.UserId == userId && j.Id == jobId)
            .FirstOrDefaultAsync();

        if (job == null)
            throw new KeyNotFoundException($"Job {jobId} not found");

        return MapToDto(job);
    }

    public async Task<IEnumerable<EnhancementJobDto>> GetUserJobsAsync(Guid userId, JobStatus? status = null)
    {
        var query = _dbContext.EnhancementJobs.Where(j => j.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return jobs.Select(MapToDto);
    }

    public async Task<bool> CanUserEnhanceAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        return user != null && CanEnhance(user);
    }

    public async Task<SubscriptionStatusDto> GetSubscriptionStatusAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        return new SubscriptionStatusDto(
            user.SubscriptionTier,
            user.FreeEnhancementsRemaining,
            user.EnhancementCredits,
            user.SubscriptionExpiresAt,
            CanEnhance(user)
        );
    }

    public async Task<bool> PurchaseCreditsAsync(Guid userId, int credits)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            return false;

        // TODO: Integrate with payment provider (Stripe, etc.)
        // For now, just add credits directly
        user.EnhancementCredits += credits;
        user.SubscriptionTier = SubscriptionTier.PayAsYouGo;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SubscribeAsync(Guid userId, SubscriptionTier tier)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            return false;

        // TODO: Integrate with payment provider
        user.SubscriptionTier = tier;
        user.SubscriptionExpiresAt = DateTime.UtcNow.AddMonths(1);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    private static bool CanEnhance(User user)
    {
        // Premium users with active subscription
        if (user.SubscriptionTier == SubscriptionTier.Premium && 
            user.SubscriptionExpiresAt > DateTime.UtcNow)
            return true;

        // Users with free enhancements remaining
        if (user.FreeEnhancementsRemaining > 0)
            return true;

        // Users with purchased credits
        if (user.EnhancementCredits > 0)
            return true;

        return false;
    }

    private static int CalculateCreditsNeeded(JobType jobType)
    {
        return jobType switch
        {
            JobType.Colorize => 1,
            JobType.RestoreQuality => 1,
            JobType.Upscale => 1,
            JobType.LightingCorrection => 1,
            JobType.SinglePhotoAnimation => 1,
            JobType.MultiPhotoMontage => 2,
            JobType.AddPersonToVideo => 2,
            JobType.ExtendVideo => 2,
            JobType.VideoUpscale => 2,
            _ => 1
        };
    }

    private static EnhancementJobDto MapToDto(EnhancementJob job)
    {
        return new EnhancementJobDto(
            job.Id,
            job.JobType,
            job.Status,
            job.ProgressPercent,
            job.StatusMessage,
            job.SourcePhotoId,
            job.SourceVideoId,
            job.ResultPhotoId,
            job.ResultVideoId,
            job.ErrorMessage,
            job.CreatedAt,
            job.CompletedAt
        );
    }
}
