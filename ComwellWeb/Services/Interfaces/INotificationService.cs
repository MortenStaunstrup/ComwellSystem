using Core;

public interface INotificationService
{
    Task SendNotificationAsync(Notification notification);
    Task<List<Notification>> GetNotificationsForUserAsync(int userId);
    Task ConfirmSubGoalCompletionAsync(int notificationId);
    Task<int> GetMaxNotificationIdAsync();

}