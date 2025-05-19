namespace Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonElement("_id")]
    public int UserId { get; set; } 
    public string UserName { get; set; }

    public List<SubGoal> StudentPlan { get; set; } = new();
   
    public List<Message> Messages { get; set; } = new();

    public ObjectId? PictureId { get; set; }
    public string? Picture { get; set; }

    public string Role { get; set; }
    public string UserPassword { get; set; }
    public string UserEmail { get; set; }
    public string UserPhone { get; set; }

    public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public int? UserIdResponsible { get; set; }
}