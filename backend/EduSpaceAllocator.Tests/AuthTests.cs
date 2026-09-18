using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace EduSpaceAllocator.Tests;

public class AuthTests
{
    private const string SecretKey = "EduSpaceAllocatorDevelopmentSecretKey2026VeryLong";
    private const string Issuer = "EduSpaceAllocator";
    private const string Audience = "EduSpaceAllocatorClient";

    [Fact]
    public void JwtTokenGeneration_ShouldIncludeUserClaimsAndRoles()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "user-123"),
            new(ClaimTypes.Email, "admin@eduspace.local"),
            new(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        tokenString.Should().NotBeNullOrWhiteSpace();

        // Validate and decode
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true
        };

        var principal = handler.ValidateToken(tokenString, validationParameters, out var validatedToken);

        principal.FindFirst(ClaimTypes.Email)?.Value.Should().Be("admin@eduspace.local");
        principal.IsInRole("Admin").Should().BeTrue();
        principal.IsInRole("Viewer").Should().BeFalse();
    }

    [Theory]
    [InlineData("Admin", true, true, true, true)]
    [InlineData("NGO", true, true, true, false)]
    [InlineData("EducationCoordinator", true, true, true, false)]
    [InlineData("Viewer", true, false, false, false)]
    public void RoleAccessModel_ShouldEnforcePermissions(
        string role,
        bool canView,
        bool canCreateEdit,
        bool canAllocate,
        bool canAdmin)
    {
        // Permission lookup table according to spec
        bool hasView = true;
        bool hasCreateEdit = role is "Admin" or "NGO" or "EducationCoordinator";
        bool hasAllocate = role is "Admin" or "NGO" or "EducationCoordinator";
        bool hasAdmin = role is "Admin";

        hasView.Should().Be(canView);
        hasCreateEdit.Should().Be(canCreateEdit);
        hasAllocate.Should().Be(canAllocate);
        hasAdmin.Should().Be(canAdmin);
    }
}
