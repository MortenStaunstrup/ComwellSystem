using Core;

namespace API.Repositories.Interface;

public interface INotificationRepository
{
    Task AddNotificationAsync(Notification notification);
    Task<List<Notification>> GetNotificationsForUserAsync(int userId);
    Task ConfirmNotificationAsync(int notificationId);
}

