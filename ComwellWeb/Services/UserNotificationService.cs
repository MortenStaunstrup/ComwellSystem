using ComwellWeb.Services.Interfaces;

namespace ComwellWeb.Services;
using Core;

public class UserNotificationService : IUserNotificationService //Bare til at begge kan bruge den. Gad ikke have dem i de andre service-mapper.
{
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    public UserNotificationService(INotificationService notificationService, IUserService userService)
    {
        _notificationService = notificationService;
        _userService = userService;
    }

    public async Task<bool> NotifyUserAsync(int userId, Notification notification)
    {
        try
        {
    
            await _notificationService.SendNotificationAsync(notification);
            
            var success = await _userService.AddNotificationToUserAsync(userId, notification);
            return success;
        }
        catch
        {
            return false;
        }
    }
}

