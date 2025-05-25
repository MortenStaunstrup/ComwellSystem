using Core;
using MongoDB.Driver;

namespace API.Repositories.Interface;

public interface INotificationRepository
{
    
    // Create
    Task SendNotificationAsync(Notification notification);
    
    // User
    Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
    
    
    // Subgoals
    Task <UpdateResult>UpdateMiniGoalAndRemoveNotificationAsync(int userId, string miniGoalName, int notificationId);
    
    
    // Id
    Task<int> GetMaxNotificationIdAsync();
    
}