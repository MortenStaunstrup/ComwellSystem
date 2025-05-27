using Core;

public interface INotificationService
{
    // Create
    Task SendMiniGoalNotificationAsync(Notification notification);

    Task SendMiddleGoalNotificationAsync(Notification notification);

    // User
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);

    // Subgoals
    Task ConfirmNotifiedMiniGoalAsync(int userId, int notificationId, string miniGoalName);

    Task ConfirmNotifiedMiddleGoalAsync(int userId, int notificationId, string middleGoalName);

    // Id
    Task<int> GetMaxNotificationIdAsync();

    Task<bool> NotificationExistsForMiniGoalAsync(int userId, int senderId, string miniGoalName);

    Task<bool> NotificationExistsForMiddleGoalAsync(int userId, int senderId, string middleGoalName);
    
}