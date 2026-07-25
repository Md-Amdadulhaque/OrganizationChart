using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrganizationChart.Api.Auth;
using OrganizationChart.Api.DTOs.Auth;
using OrganizationChart.Api.Services.Interfaces;
using OrganizationChart.Domain.Models;

namespace OrganizationChart.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var account = await _context.AuthUsers
            .FirstOrDefaultAsync(a => a.Username == dto.Username);

        if (account == null)
            return null;

        if (!PasswordHasher.Verify(dto.Password, account.PasswordHash))
            return null;

        return BuildResponse(account);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var exists = await _context.AuthUsers
            .AnyAsync(a => a.Username == dto.Username);

        if (exists)
            throw new InvalidOperationException($"Username '{dto.Username}' is already taken.");

        var role = dto.Role == Roles.Admin ? Roles.Admin : Roles.Viewer;

        var account = new AuthUser
        {
            Username = dto.Username,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _context.AuthUsers.Add(account);
        await _context.SaveChangesAsync();

        return BuildResponse(account);
    }

    public async Task SeedAdminAsync()
    {
        if (await _context.AuthUsers.AnyAsync())
            return;

        var username = _config["SeedAdmin:Username"] ?? "admin";
        var password = _config["SeedAdmin:Password"] ?? "Admin@123";

        _context.AuthUsers.Add(new AuthUser
        {
            Username = username,
            PasswordHash = PasswordHasher.Hash(password),
            Role = Roles.Admin,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    private AuthResponseDto BuildResponse(AuthUser account)
    {
        var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var expiresMinutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 480;

        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.Username),
            new Claim(ClaimTypes.Role, account.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            Token = tokenString,
            Username = account.Username,
            Role = account.Role,
            ExpiresAt = expiresAt
        };
    }
}
