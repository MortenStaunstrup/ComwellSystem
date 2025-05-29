using Core;
using MongoDB.Driver;

namespace API.Repositories.Interface;

public interface INotificationRepository
{
    
    // Afsendelse
    Task SendMiniGoalNotificationAsync(Notification notification); // når eleven markerer et minigoal færdigt, bliver en notifikation sendt.
    
    Task SendMiddleGoalNotificationAsync(Notification notification); // når eleven markerer et middlegoal færdigt, eller alle minigoals i middlegoal er færdigt, bliver en notifikation sendt.
    
    // Users (blev erstattet af GetUserByUserId).
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
    
    
    // Goals
    Task<bool> UpdateMiniGoalStatusAsync(int studentId, string miniGoalName); // sætter status = true på minigoal når det er blevet bekræftet.
    Task<bool> UpdateMiddleGoalStatusAsync(int studentId, string middleGoalName); // sætter status = true på middlegoal når det er blevet bekræftet.
    Task<bool> RemoveNotificationMiniGoalFromManagerAsync(int leaderId, int notificationId); // fjerne embedded minigoal-notification når bekræftet.
    
    Task<bool> RemoveNotificationMiddleGoalFromManagerAsync(int leaderId, int notificationId); // fjerne embedded middlegoal-notification når bekræftet.
    Task<bool> NotificationExistsForMiniGoalAsync(int userId, int senderId, string miniGoalName); // tjekker om eleven allerede har afsendt samme notifikation til samme minigoal. Forebygger duplikationer
    Task<bool> NotificationExistsForMiddleGoalAsync(int userId, int senderId, string middleGoalName); // tjekker om eleven allerede har afsendt samme notifikation til samme middlegoal. Forebygger duplikationer
    
    // Id
    Task<int> GetMaxNotificationIdAsync(); // næste notifikations ID bliver +1 højere end sidste.
}