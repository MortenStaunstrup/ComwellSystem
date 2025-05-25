using API.Repositories.Interface;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(IUserRepository userRepository, INotificationRepository notificationRepository)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }


        // Opretter notification ved at opdatere user dokument (embedde notification)
        [HttpPost("send")]
        public async Task<IActionResult> SendNotificationAsync([FromBody] Notification notification)
        {
            if (notification == null || notification.UserId == null || notification.UserId == 0)
                return BadRequest("Invalid notification data: UserId is required");

            try
            {
                {
                    if (string.IsNullOrWhiteSpace(notification.MiniGoalName))
                        return BadRequest("MiniGoalName is required");

                }
                await _notificationRepository.SendNotificationAsync(notification);
                return Ok("Notification sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught in controller: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            
        }



        [HttpGet("notifications/user/{userId}")]
        public async Task<IActionResult> GetNotificationsByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByUserId(userId);
            if (user == null)
                return NotFound("User not found");

            var notifications = user.Notifications;
            if (notifications == null || !notifications.Any())
                return NotFound("Ingen notifikationer fundet for denne bruger.");

            return Ok(notifications);
        }

        // Bekræfter mini goal ud fra notificationId (embedded notifications i user)
        [HttpPost("confirm/{userId}/{notificationId}")]
        public async Task<IActionResult> ConfirmSubGoalNotificationAsync(int userId, int notificationId)
        {
            var user = await _userRepository.GetUserByUserId(userId);
            if (user == null)
                return NotFound("User not found");

            var notification = user.Notifications.FirstOrDefault(n => n.NotificationId == notificationId);
            if (notification == null)
                return NotFound("Notification not found");

            // Antager notification indeholder MiniGoalName - eller send det med payload
            string miniGoalName = notification.NotificationContent; // eller tilpas

            await _userRepository.ConfirmMiniGoalAsync(userId, miniGoalName);

            // Fjern bekræftet notifikation (valgfrit)
            user.Notifications.Remove(notification);
            await _userRepository.UpdateUserAsync(user);

            return Ok("Notification confirmed and mini goal updated");
        }
        [HttpGet("maxid")]
        public async Task<ActionResult<int>> GetMaxNotificationIdAsync()
        {
            int maxId = await _notificationRepository.GetMaxNotificationIdAsync();
            return Ok(maxId);
        }

    }
}
