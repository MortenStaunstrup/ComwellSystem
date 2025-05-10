using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core;

public class Message
{
    [BsonElement("_id")]
    public int MessageId { get; set; }
    public ObjectId? FileId { get; set; }
    public string MessageContent { get; set; }
    public DateTime MessageDate { get; set; }
    public int MessegeSenderId { get; set; }
    public int? MessegeReceiverId { get; set; }
    
}