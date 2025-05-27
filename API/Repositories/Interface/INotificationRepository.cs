using Core;
using MongoDB.Driver;

namespace API.Repositories.Interface;

public interface INotificationRepository
{
    
    // Create
    Task SendMiniGoalNotificationAsync(Notification notification);
    
    Task SendMiddleGoalNotificationAsync(Notification notification);
    
    // User
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
    
    
    // Subgoals
    Task <UpdateResult>UpdateMiniGoalAndRemoveNotificationAsync(int userId, string miniGoalName, int notificationId);
    
    Task <UpdateResult>UpdateMiddleGoalAndRemoveNotificationAsync(int userId, string miniGoalName, int notificationId);
    
    
    // Id
    Task<int> GetMaxNotificationIdAsync();

    Task<bool> NotificationExistsForMiddleGoalAsync(int userId, int senderId, string middleGoalName);

    Task<bool> NotificationExistsForMiniGoalAsync(int userId, int senderId, string miniGoalName);

}