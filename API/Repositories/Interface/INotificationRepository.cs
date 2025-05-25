using Core;

namespace API.Repositories.Interface;

public interface INotificationRepository
{
    
    // Create
    Task SendNotificationAsync(Notification notification);
    
    // User
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
    
    
    // Subgoals
    Task ConfirmNotifiedSubgoalAsync(int notificationId);
    
    
    // Id
    Task<int> GetMaxNotificationIdAsync();
    
}