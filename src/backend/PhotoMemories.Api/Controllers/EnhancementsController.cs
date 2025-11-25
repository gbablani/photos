using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoMemories.Api.DTOs;
using PhotoMemories.Api.Models;
using PhotoMemories.Api.Services;

namespace PhotoMemories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnhancementsController : ControllerBase
{
    private readonly IEnhancementService _enhancementService;

    public EnhancementsController(IEnhancementService enhancementService)
    {
        _enhancementService = enhancementService;
    }

    /// <summary>
    /// Get subscription status and remaining credits
    /// </summary>
    [HttpGet("subscription")]
    public async Task<ActionResult<SubscriptionStatusDto>> GetSubscriptionStatus()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var status = await _enhancementService.GetSubscriptionStatusAsync(userId.Value);
            return Ok(status);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Purchase enhancement credits
    /// </summary>
    [HttpPost("purchase-credits")]
    public async Task<ActionResult> PurchaseCredits([FromBody] PurchaseCreditsDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        // Validate credit package
        var validPackages = new[] { 10, 25, 50, 100 };
        if (!validPackages.Contains(request.CreditPackage))
            return BadRequest("Invalid credit package. Valid options: 10, 25, 50, 100");

        var success = await _enhancementService.PurchaseCreditsAsync(userId.Value, request.CreditPackage);
        if (!success)
            return BadRequest("Failed to purchase credits");

        return Ok(new { message = $"Successfully purchased {request.CreditPackage} credits" });
    }

    /// <summary>
    /// Subscribe to premium tier
    /// </summary>
    [HttpPost("subscribe")]
    public async Task<ActionResult> Subscribe([FromBody] SubscribeDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        if (request.Tier != SubscriptionTier.Premium)
            return BadRequest("Invalid subscription tier");

        var success = await _enhancementService.SubscribeAsync(userId.Value, request.Tier);
        if (!success)
            return BadRequest("Failed to subscribe");

        return Ok(new { message = "Successfully subscribed to Premium" });
    }

    /// <summary>
    /// Create a new enhancement job
    /// </summary>
    [HttpPost("jobs")]
    public async Task<ActionResult<EnhancementJobDto>> CreateJob([FromBody] CreateEnhancementJobDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var job = await _enhancementService.CreateJobAsync(userId.Value, request);
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("User not found");
        }
    }

    /// <summary>
    /// Get a specific enhancement job
    /// </summary>
    [HttpGet("jobs/{id}")]
    public async Task<ActionResult<EnhancementJobDto>> GetJob(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var job = await _enhancementService.GetJobAsync(userId.Value, id);
            return Ok(job);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Get all enhancement jobs for the current user
    /// </summary>
    [HttpGet("jobs")]
    public async Task<ActionResult<IEnumerable<EnhancementJobDto>>> GetJobs([FromQuery] JobStatus? status)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var jobs = await _enhancementService.GetUserJobsAsync(userId.Value, status);
        return Ok(jobs);
    }

    /// <summary>
    /// Colorize a black and white photo
    /// </summary>
    [HttpPost("colorize/{photoId}")]
    public async Task<ActionResult<EnhancementJobDto>> ColorizePhoto(Guid photoId)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var request = new CreateEnhancementJobDto(JobType.Colorize, photoId, null, null, null);
            var job = await _enhancementService.CreateJobAsync(userId.Value, request);
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Restore and enhance photo quality
    /// </summary>
    [HttpPost("restore/{photoId}")]
    public async Task<ActionResult<EnhancementJobDto>> RestorePhoto(Guid photoId)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var request = new CreateEnhancementJobDto(JobType.RestoreQuality, photoId, null, null, null);
            var job = await _enhancementService.CreateJobAsync(userId.Value, request);
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Animate a single photo into a video clip
    /// </summary>
    [HttpPost("animate/{photoId}")]
    public async Task<ActionResult<EnhancementJobDto>> AnimatePhoto(Guid photoId, [FromBody] AnimatePhotoRequest? options)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var enhanceOptions = options != null 
                ? new EnhancementOptions(options.Style, options.AddMusic, options.DurationSeconds, null)
                : null;
            var request = new CreateEnhancementJobDto(JobType.SinglePhotoAnimation, photoId, null, null, enhanceOptions);
            var job = await _enhancementService.CreateJobAsync(userId.Value, request);
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Create a montage video from multiple photos
    /// </summary>
    [HttpPost("montage")]
    public async Task<ActionResult<EnhancementJobDto>> CreateMontage([FromBody] CreateMontageRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        if (request.PhotoIds == null || request.PhotoIds.Count < 2)
            return BadRequest("At least 2 photos are required for a montage");

        try
        {
            var enhanceOptions = new EnhancementOptions(request.Style, request.AddMusic, request.DurationSeconds, null);
            var jobRequest = new CreateEnhancementJobDto(
                JobType.MultiPhotoMontage, 
                request.PhotoIds[0], 
                null, 
                request.PhotoIds.Skip(1).ToList(), 
                enhanceOptions
            );
            var job = await _enhancementService.CreateJobAsync(userId.Value, jobRequest);
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
}

public record AnimatePhotoRequest(AnimationStyle? Style, bool? AddMusic, int? DurationSeconds);
public record CreateMontageRequest(List<Guid> PhotoIds, AnimationStyle? Style, bool? AddMusic, int? DurationSeconds);
