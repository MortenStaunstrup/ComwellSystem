using Core;

public interface INotificationService
{
    // Create
    Task SendNotificationAsync(Notification notification);
    
    // User
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
    
    // Subgoals
    Task ConfirmNotifiedSubGoalAsync(int userId, int notificationId, string miniGoalName);
    
    // Id
    Task<int> GetMaxNotificationIdAsync();
}