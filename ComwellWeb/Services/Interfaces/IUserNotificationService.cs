namespace ComwellWeb.Services.Interfaces;
using Core;

public interface IUserNotificationService
{
    Task<bool> NotifyUserAsync(int userId, Notification notification);

}