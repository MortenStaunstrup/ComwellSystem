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
    Task<bool> UpdateMiniGoalStatusAsync(int studentId, string miniGoalName);
    Task<bool> UpdateMiddleGoalStatusAsync(int studentId, string middleGoalName);
    Task<bool> RemoveNotificationMiniGoalFromManagerAsync(int leaderId, int notificationId);
    
    Task<bool> RemoveNotificationMiddleGoalFromManagerAsync(int leaderId, int notificationId);


    
    
    // Id
    Task<int> GetMaxNotificationIdAsync();

    Task<bool> NotificationExistsForMiddleGoalAsync(int userId, int senderId, string middleGoalName);

    Task<bool> NotificationExistsForMiniGoalAsync(int userId, int senderId, string miniGoalName);

}