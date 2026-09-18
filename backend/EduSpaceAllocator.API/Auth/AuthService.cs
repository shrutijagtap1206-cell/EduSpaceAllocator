using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EduSpaceAllocator.API.Auth;

public class AuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Message, string? Token, IList<string>? Roles, string? Email)> LoginAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return (false, "Invalid email or password.", null, null, null);

        if (!await _userManager.CheckPasswordAsync(user, password))
            return (false, "Invalid email or password.", null, null, null);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? email),
            new(ClaimTypes.Name, user.UserName ?? email)
        };

        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiryMinutes = double.TryParse(_configuration["Jwt:ExpiryMinutes"], out var exp) ? exp : 120;

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "EduSpaceAllocator",
            audience: _configuration["Jwt:Audience"] ?? "EduSpaceAllocatorClient",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return (
            true,
            "Login successful.",
            new JwtSecurityTokenHandler().WriteToken(token),
            roles,
            user.Email);
    }
}

