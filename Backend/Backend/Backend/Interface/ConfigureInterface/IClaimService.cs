using Backend.Backend.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Backend.Interface.ConfigureInterface
{
    public interface IClaimService
    {
        Task<List<Claim>> GetClaimsAsync(User user);
    }
}
