using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhotoMemories.Api.Data;
using PhotoMemories.Api.DTOs;
using PhotoMemories.Api.Models;

namespace PhotoMemories.Api.Services;

public interface IUserService
{
    Task<UserDto> GetUserAsync(Guid userId);
    Task<UserDto> GetOrCreateUserAsync(AuthProvider provider, string externalId, string email, string displayName, string? profilePicture);
    Task<UserDto> UpdateUserAsync(Guid userId, UserProfileUpdateDto update);
    Task<AuthResponseDto> GenerateAuthTokensAsync(User user);
    Task<User?> ValidateRefreshTokenAsync(string refreshToken);
}

public class UserService : IUserService
{
    private readonly PhotoMemoriesDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public UserService(PhotoMemoriesDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<UserDto> GetUserAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        return MapToDto(user);
    }

    public async Task<UserDto> GetOrCreateUserAsync(AuthProvider provider, string externalId, string email, string displayName, string? profilePicture)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.AuthProvider == provider && u.ExternalId == externalId);

        if (user == null)
        {
            // Check if email already exists with different provider
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Email {email} is already registered with a different provider");
            }

            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                DisplayName = displayName,
                ProfilePictureUrl = profilePicture,
                AuthProvider = provider,
                ExternalId = externalId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                FreeEnhancementsRemaining = 2  // Initial free enhancements
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            // Update user info if changed
            user.DisplayName = displayName;
            user.Email = email;
            if (!string.IsNullOrEmpty(profilePicture))
                user.ProfilePictureUrl = profilePicture;
            user.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        return MapToDto(user);
    }

    public async Task<UserDto> UpdateUserAsync(Guid userId, UserProfileUpdateDto update)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (update.DisplayName != null)
            user.DisplayName = update.DisplayName;

        if (update.AutoSyncEnabled.HasValue)
            user.AutoSyncEnabled = update.AutoSyncEnabled.Value;

        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<AuthResponseDto> GenerateAuthTokensAsync(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "PhotoMemories";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "PhotoMemoriesApp";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiresAt = DateTime.UtcNow.AddHours(24);
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return await Task.FromResult(new AuthResponseDto(
            accessToken,
            refreshToken,
            MapToDto(user),
            expiresAt
        ));
    }

    public async Task<User?> ValidateRefreshTokenAsync(string refreshToken)
    {
        // TODO: Implement proper refresh token storage and validation
        // For now, just return null (refresh tokens not fully implemented)
        await Task.CompletedTask;
        return null;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.Email,
            user.DisplayName,
            user.ProfilePictureUrl,
            user.SubscriptionTier,
            user.FreeEnhancementsRemaining,
            user.EnhancementCredits,
            user.SubscriptionExpiresAt,
            user.GooglePhotosConnected,
            user.OneDriveConnected,
            user.AutoSyncEnabled
        );
    }
}
