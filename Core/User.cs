using MongoDB.Bson.Serialization.Attributes;

namespace Core;

public class User
{
    [BsonElement("_id")]
    public int UserId { get; set; }
    public string UserName { get; set; }
    public SubGoal[]? StudentPlan { get; set; }
    public Notification[]? Notifications { get; set; }
    public Message[]? Messages { get; set; }
    public string? Picture { get; set; }
    public string Role { get; set;}
    public string UserPassword { get; set; }
    public string UserEmail { get; set; }
    public string UserPhone { get; set; }
    public DateOnly? UserSemester { get; set; }
    public int? UserIdResponsible { get; set; }
}