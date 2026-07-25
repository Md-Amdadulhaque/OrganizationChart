using OrganizationChart.Api.DTOs.Auth;

namespace OrganizationChart.Api.Services.Interfaces;

public interface IAuthService
{
    /// <summary>Returns a token response on success, or null on bad credentials.</summary>
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);

    /// <summary>Creates a new account. Throws InvalidOperationException if the username is taken.</summary>
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

    /// <summary>Creates the default admin account if no accounts exist yet.</summary>
    Task SeedAdminAsync();
}
