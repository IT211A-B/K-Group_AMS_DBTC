using Backend.Backend.Interface.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Backend.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountHistoryController : ControllerBase
    {
        private readonly IAccountActivityService _activityService;

        public AccountHistoryController(IAccountActivityService activityService) => _activityService = activityService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _activityService.GetRecentAsync();
            return Ok(result.Data ?? []);
        }
    }
}
