namespace Core;

public class Notification
{
    public int NotificationId { get; set; } // PK
    public int? UserId { get; set; } // FK
    public int? SenderId { get; set; } // FK
    public string MiniGoalName { get; set; } // FK
    public string MiddleGoalName { get; set; }
    public string NotificationContent { get; set; }
    
    public DateTime TimeStamp { get; set; }
    
    public bool IsConfirmed { get; set; }
}

public class LastestNoti
{
    public List<Notification> Notifications30 { get; set; }

    public LastestNoti(List<Notification> allNotifications)
    {
        Notifications30 = allNotifications
            .Where(n => (DateTime.Now - n.TimeStamp).TotalDays <= 30)
            .ToList();
    }
}