namespace ComwellWeb.Services.Interfaces;
using Core;

public interface IUserNotificationService
{
    // Task<bool> NotifyUserAsync(int userId, Notification notification);
    Task <bool> IsNotificationSent(int userId, Notification notification);
    

}