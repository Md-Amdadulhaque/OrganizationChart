namespace OrganizationChart.Domain.Models;

/// <summary>
/// A login account for the application. Kept separate from the imported
/// employee <see cref="User"/> records (which have no credentials).
/// </summary>
public class AuthUser
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    /// <summary>PBKDF2 hash, stored as "iterations.salt.subkey" (base64 parts).</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>"Admin" or "Viewer".</summary>
    public string Role { get; set; } = Roles.Viewer;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string Viewer = "Viewer";
}
