using System.Net;
using System.Net.Http.Json;
using Backend.Backend.DTOs;
using Xunit;

namespace Backend.Tests;

public class RateLimitIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RateLimitIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Returns429_WhenAuthRateLimitExceeded()
    {
        var payload = new LoginUserDto
        {
            Email = "nobody@admin.local",
            Password = "wrong-password"
        };

        var statuses = new List<HttpStatusCode>();
        for (var i = 0; i < 6; i++)
        {
            var response = await _client.PostAsJsonAsync("/LogIn", payload);
            statuses.Add(response.StatusCode);
        }

        Assert.Contains(HttpStatusCode.TooManyRequests, statuses);
    }

    [Fact]
    public async Task Swagger_IsReachable()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
