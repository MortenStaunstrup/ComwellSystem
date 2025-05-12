using MongoDB.Bson.Serialization.Attributes;

namespace Core;

public class Notification
{
    [BsonElement("_id")]
    public int NotificationId { get; set; }
    public int NotificationUserId { get; set; }
    public string NotificationContent { get; set; }
}