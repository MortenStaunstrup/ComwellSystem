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

        // Elev sender notifikation om færdig opgave til køkkenleder
        [HttpPost("send")]
        public async Task<IActionResult> SendNotificationAsync([FromBody] Notification notification)
        {
            if (notification == null || notification.ReceiverUserId == 0)
                return BadRequest("Invalid notification data");

            await _notificationRepository.AddNotificationAsync(notification);
            return Ok("Notification sent");
        }

        // Køkkenleder henter sine notifikationer
        [HttpGet("notifications/user/{userId}")]
        public async Task<IActionResult> GetNotificationsForUser(int userId)
        {
            var notifications = await _notificationRepository.GetNotificationsForUserAsync(userId);

            if (notifications == null || !notifications.Any())
            {
                return NotFound("Ingen notifikationer fundet for denne bruger.");
            }

            return Ok(notifications);
        }

        // Køkkenleder bekræfter en notifikation
        [HttpPost("confirm/{notificationId}")]
        public async Task<IActionResult> ConfirmNotification(int notificationId)
        {
            await _notificationRepository.ConfirmNotifiedSubgoalAsync(notificationId);
            return Ok("Notification confirmed");
        }
        [HttpGet("maxid")]
        public async Task<ActionResult<int>> GetMaxNotificationId()
        {
            int maxId = await _notificationRepository.GetMaxNotificationIdAsync();
            return Ok(maxId);
        }

        
    }
    
}
