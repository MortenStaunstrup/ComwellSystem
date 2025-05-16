using API.Repositories.Interface;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        // Student sender notifikation om færdig opgave til køkkenleder
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] Notification notification)
        {
            if (notification == null || notification.ReceiverUserId == 0)
                return BadRequest("Invalid notification data");

            await _notificationRepository.AddNotificationAsync(notification);
            return Ok("Notification sent");
        }

        // Køkkenleder henter sine notifikationer
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetNotificationsForUser(int userId)
        {
            var notifications = await _notificationRepository.GetNotificationsForUserAsync(userId);
            return Ok(notifications);
        }

        // Køkkenleder bekræfter en notifikation
        [HttpPost("confirm/{notificationId}")]
        public async Task<IActionResult> ConfirmNotification(int notificationId)
        {
            await _notificationRepository.ConfirmNotificationAsync(notificationId);
            return Ok("Notification confirmed");
        }
    }
}
