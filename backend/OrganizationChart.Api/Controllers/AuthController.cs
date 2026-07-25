using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationChart.Api.DTOs.Auth;
using OrganizationChart.Api.Services.Interfaces;
using OrganizationChart.Domain.Models;

namespace OrganizationChart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result == null)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(result);
    }

    // Only admins can create new accounts.
    [Authorize(Roles = Roles.Admin)]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // Returns the current caller's identity (used by the UI to restore session).
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            username = User.Identity?.Name,
            role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
        });
    }
}
