using SharpEngine.Shared.Dto.Primitives;

namespace SharpEngine.Shared.Dto;

/// <summary>
///     Represents a user profile in the SharpEngine system.
/// </summary>
public class UserProfile
{
    public UserId UserId { get; init; }
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Auth0Identifier { get; init; }

    // TODO: What user details do we need from Auth0?

    public List<SocialMedia> LinkedSocialMedia { get; init; } = [];

    public List<Project> Projects { get; init; } = [];

    public List<Achievement> Achievements { get; init; } = [];
}
