using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Backend.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService) => _mailService = mailService;

        private string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        private string Role => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        [HttpGet("Notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            if (string.IsNullOrEmpty(UserId)) return Unauthorized();
            var result = await _mailService.GetNotificationsAsync(UserId, Role);
            return Ok(result.Data ?? Enumerable.Empty<GetMailDTO>());
        }

        [HttpPost("Notifications")]
        public async Task<IActionResult> Send([FromBody] SendMailDTO dto)
        {
            if (string.IsNullOrEmpty(UserId)) return Unauthorized();
            var name = User.FindFirst(ClaimTypes.Name)?.Value ?? "User";
            var result = await _mailService.SendAsync(dto, UserId, name, Role);
            return Ok(result.Data);
        }

        [HttpPut("Notifications/{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            await _mailService.MarkReadAsync(id);
            return Ok();
        }

        [HttpPut("Notifications/read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            if (string.IsNullOrEmpty(UserId)) return Unauthorized();
            await _mailService.MarkAllReadAsync(UserId);
            return Ok();
        }

        [HttpDelete("Notifications")]
        public async Task<IActionResult> Clear()
        {
            if (string.IsNullOrEmpty(UserId)) return Unauthorized();
            await _mailService.ClearAsync(UserId);
            return Ok();
        }
    }
}
