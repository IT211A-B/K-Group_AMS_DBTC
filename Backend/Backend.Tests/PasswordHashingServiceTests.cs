using Backend.Backend.Configuration;
using Backend.Backend.Model;
using Xunit;

namespace Backend.Tests;

public class PasswordHashingServiceTests
{
    private readonly PasswordHashingService _service = new();

    [Fact]
    public void HashPassword_ProducesDifferentValueThanPlainText()
    {
        var user = new User
        {
            DocumentSeries = "TST-2026-1",
            Full_Name = "Test User",
            UserName = "test@admin.local",
            Email = "test@admin.local"
        };

        var hash = _service.HashPassword(user, "SecretPass123!");

        Assert.NotEqual("SecretPass123!", hash);
        Assert.False(string.IsNullOrWhiteSpace(hash));
    }

    [Fact]
    public void VerifyPassword_ReturnsTrueForCorrectPassword()
    {
        var user = new User
        {
            DocumentSeries = "TST-2026-2",
            Full_Name = "Test User",
            UserName = "verify@admin.local",
            Email = "verify@admin.local"
        };

        const string password = "AnotherSecret1!";
        var hash = _service.HashPassword(user, password);

        Assert.True(_service.VerifyPassword(user, hash, password));
    }

    [Fact]
    public void VerifyPassword_ReturnsFalseForWrongPassword()
    {
        var user = new User
        {
            DocumentSeries = "TST-2026-3",
            Full_Name = "Test User",
            UserName = "wrong@admin.local",
            Email = "wrong@admin.local"
        };

        var hash = _service.HashPassword(user, "CorrectPassword1!");

        Assert.False(_service.VerifyPassword(user, hash, "WrongPassword1!"));
    }
}
