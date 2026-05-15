using System.Security.Claims;
using Backend.Backend.Configuration;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Backend.Tests;

public class JwtServiceTests
{
    [Fact]
    public void GenerateToken_ReturnsNonEmptyJwt()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-jwt-signing-key-at-least-32-characters-long",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience",
                ["Jwt:ExpireMinutes"] = "60"
            })
            .Build();

        var service = new JwtService(config);
        var token = service.GenerateToken(new List<Claim>
        {
            new(ClaimTypes.Name, "test-user"),
            new(ClaimTypes.Role, "Admin")
        });

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(3, token.Split('.').Length);
    }
}
