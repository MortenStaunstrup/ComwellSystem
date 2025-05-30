﻿using API.Repositories.Interface;
using Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

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
        [HttpPost("send-mini-goal")]
        public async Task<IActionResult> SendMiniGoalNotificationAsync([FromBody] Notification notification)
        {
            if (notification == null || notification.UserId == null || notification.UserId == 0)
                return BadRequest("Invalid notification data: UserId is required");

            try
            {
                {
                    if (string.IsNullOrWhiteSpace(notification.MiniGoalName))
                        return BadRequest("MiniGoalName is required");

                }
                await _notificationRepository.SendMiniGoalNotificationAsync(notification);
                return Ok("Notification sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught in controller: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        /// <summary>
        /// opretter og sender en middlegoal-notifikation til en bruger
        /// </summary>
        /// <param name="notification">notifikation der skal sendes</param>
        /// <returns>http 200 ved succes, ellers relevant fejlbesked</returns>
        [HttpPost]
        [Route("send-middle-goal")]
        public async Task<IActionResult> SendMiddleGoalNotificationAsync([FromBody] Notification notification)
        {
            if (notification == null || notification.UserId == null || notification.UserId == 0)
                return BadRequest("Invalid notification data: UserId is required");

            try
            {
                {
                    if (string.IsNullOrWhiteSpace(notification.MiddleGoalName))
                        return BadRequest("MiddleGoalName is required");

                }
                await _notificationRepository.SendMiddleGoalNotificationAsync(notification);
                return Ok("Notification sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught in controller: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


        /// <summary>
        /// henter alle notifikationer for en given bruger
        /// </summary>
        /// <param name="userId">userid for brugeren</param>
        /// <returns>liste af notifikationer</returns>
        [HttpGet("notifications/user/{userId}")]
        public async Task<IActionResult> GetNotificationsByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByUserId(userId);
            if (user == null)
                return NotFound("User not found");

            var notifications = user.Notifications;
            if (notifications == null || !notifications.Any())
            {
                Console.WriteLine("No notifications found");
                return Ok(new List<Notification>());
            }

            return Ok(notifications);
        }

        /// <summary>
        /// bekræfter en minigoalnotifikation og opdaterer status på målet
        /// </summary>
        /// <param name="userId">userid for lederen</param>
        /// <param name="notificationId">id på den notifikation der bekræftes</param>
        /// <param name="miniGoalName">navn på det tilhørende minigoal</param>
        /// <returns>http 200 ved succes, ellers fejl</returns>
        [HttpPost("confirm-mini-goal/{userId}/{notificationId}/{miniGoalName}")]
        public async Task<IActionResult> ConfirmMiniGoalNotificationAsync(int userId, int notificationId, string miniGoalName)
        {
            var leader = await _userRepository.GetUserByUserId(userId);
            if (leader == null) return NotFound("Leader not found");

            var notification = leader.Notifications
                .FirstOrDefault(n =>
                    n.NotificationId == notificationId &&
                    n.MiniGoalName?.Trim().Equals(miniGoalName.Trim(), StringComparison.OrdinalIgnoreCase) == true);

            if (notification == null)
                return NotFound("Notification not found or mismatched MiniGoalName");

            var studentId = notification.SenderId ?? 0;
            if (studentId == 0)
                return BadRequest("Missing sender/student ID");

            var statusUpdated = await _notificationRepository.UpdateMiniGoalStatusAsync(studentId, miniGoalName);
            var notificationRemoved = await _notificationRepository.RemoveNotificationMiniGoalFromManagerAsync(userId, notificationId);

            if (!statusUpdated || !notificationRemoved)
                return BadRequest("Failed to update mini goal or remove notification");

            return Ok("Notification confirmed and mini goal updated");
        }


        /// <summary>
        /// bekræfter en middlegoal notifikation og opdaterer status på målet. Ligesom minigoal overstående
        /// </summary>
        /// <param name="leaderId">userid for lederen/kitchenmanager</param>
        /// <param name="notificationId">id på notifikationen</param>
        /// <param name="middleGoalName">navn på målet</param>
        /// <returns>http 200 ved succes, ellers fejl</returns>
        [HttpPost("confirm-middle-goal/{leaderId}/{notificationId}/{middleGoalName}")]
        public async Task<IActionResult> ConfirmMiddleGoalNotificationAsync(int leaderId, int notificationId, string middleGoalName)
        {
            var leader = await _userRepository.GetUserByUserId(leaderId);
            if (leader == null) return NotFound("Leader not found");

            var notification = leader.Notifications.FirstOrDefault(n =>
                n.NotificationId == notificationId &&
                n.MiddleGoalName?.Trim().Equals(middleGoalName.Trim(), StringComparison.OrdinalIgnoreCase) == true);

            if (notification == null)
                return NotFound("Notification not found or mismatched MiddleGoalName");

            var studentId = notification.SenderId ?? 0;
            if (studentId == 0)
                return BadRequest("Missing sender/student ID");

            var goalUpdated = await _notificationRepository.UpdateMiddleGoalStatusAsync(studentId, middleGoalName);
            var notificationRemoved = await _notificationRepository.RemoveNotificationMiddleGoalFromManagerAsync(leaderId, notificationId);

            if (!goalUpdated || !notificationRemoved)
                return BadRequest("Goal update or notification removal failed");

            return Ok("Middle goal confirmed and notification removed");
        }
        
        /// <summary>
        /// returnerer det højeste notificationid i systemet
        /// </summary>
        /// <returns>heltal som repræsenterer det største eksisterende id</returns>
    [HttpGet("maxid")]
    public async Task<ActionResult<int>> GetMaxNotificationIdAsync()
{
    int maxId = await _notificationRepository.GetMaxNotificationIdAsync();
    return Ok(maxId);
}

        /// <summary>
        /// tjekker om en notifikation for et specifikt minigoal allerede findes
        /// </summary>
        /// <param name="userId">userid for modtageren</param>
        /// <param name="senderId">userid for afsenderen</param>
        /// <param name="miniGoalName">navn på minigoalet</param>
        /// <returns>true hvis den findes, ellers false</returns>
    [HttpGet("exists-mini-goal")]
    public async Task<ActionResult<bool>> NotificationExistsForMiniGoal([FromQuery] int userId, [FromQuery] int senderId, [FromQuery] string miniGoalName) 
    { 
            try
            {
                var exists = await _notificationRepository.NotificationExistsForMiniGoalAsync(userId, senderId, miniGoalName);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fejl ved opslag af mini-mål notifikation: {ex.Message}");
            }
        }
        
        /// <summary>
        /// tjekker om en notifikation for et specifikt middlegoal allerede findes
        /// </summary>
        /// <param name="userId">userid for modtageren</param>
        /// <param name="senderId">userid for afsenderen</param>
        /// <param name="middleGoalName">navn på middlegoalet</param>
        /// <returns>true hvis den findes, ellers false</returns>
        [HttpGet("exists-middle-goal")]
        public async Task<ActionResult<bool>> NotificationExistsForMiddleGoal(
            [FromQuery] int userId,
            [FromQuery] int senderId,
            [FromQuery] string middleGoalName)
        {
            try
            {
                var exists = await _notificationRepository.NotificationExistsForMiddleGoalAsync(userId, senderId, middleGoalName);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fejl ved opslag af middel-mål notifikation: {ex.Message}");
            }
        }

    }
}
