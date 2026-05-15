using Backend.Backend.Helper;
using PosStatus = Backend.Backend.Helper.Enum.PosEnum.PosStatus;
using Xunit;

namespace Backend.Tests;

public class AddRoleTests
{
    [Theory]
    [InlineData("student@dbtc-cebu.edu", PosStatus.STU, 200)]
    [InlineData("teacher@local.edu", PosStatus.TEA, 200)]
    [InlineData("admin@admin.local", PosStatus.ADM, 200)]
    [InlineData("unknown@example.com", null, 404)]
    public void AddRoleAccordingToEmail_MapsDomains(string email, PosStatus? expectedRole, int expectedStatus)
    {
        var (role, statusCode) = email.AddRoleAccordingToEmail();

        Assert.Equal(expectedRole, role);
        Assert.Equal(expectedStatus, statusCode);
    }
}
