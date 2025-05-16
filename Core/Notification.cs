
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{
    public class Notification
    {
        [BsonId]
        public int Id { get; set; }

        
        public int NotificationId { get; set; } // valgfrit: eller brug bare string Id
        public int SubGoalId { get; set; }
        public int SenderUserId { get; set; }
        public int? ReceiverUserId { get; set; }
        public string Message { get; set; } = "";
        public bool IsConfirmed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsCconfirmed { get; set; }
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
}