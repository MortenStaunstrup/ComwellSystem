namespace Core;

public class Notification
{
    public int NotificationId { get; set; }
    public int NotificationUserId { get; set; }
    public string NotificationContent { get; set; }
    public DateTime NotificationDate { get; set; }
}

public class LastestNoti
{
    public List<Notification> Notifications30 { get; set; }

    public LastestNoti(List<Notification> allNotifications)
    {
        Notifications30 = allNotifications
            .Where(n => (DateTime.Now - n.NotificationDate).TotalDays <= 30)
            .ToList();
    }
}