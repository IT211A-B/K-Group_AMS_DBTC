using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Backend.Interface.ConfigureInterface
{
    public interface IJwtService
    {
        string GenerateToken(List<Claim> claims);
    }
}
