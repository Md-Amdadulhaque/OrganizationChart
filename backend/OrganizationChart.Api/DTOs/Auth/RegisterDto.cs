using System.ComponentModel.DataAnnotations;

namespace OrganizationChart.Api.DTOs.Auth;

public class RegisterDto
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>"Admin" or "Viewer". Defaults to "Viewer" when omitted.</summary>
    public string Role { get; set; } = "Viewer";
}
