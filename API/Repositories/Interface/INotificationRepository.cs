using Core;

namespace API.Repositories.Interface;

public interface INotificationRepository
{
    
    // Create
    Task SendNotificationAsync(Notification notification);
    
    // User
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
    
    
    // Subgoals
    Task ConfirmMiniGoalAsync(int? userId, string miniGoalName);
    
    
    // Id
    Task<int> GetMaxNotificationIdAsync();
    
}