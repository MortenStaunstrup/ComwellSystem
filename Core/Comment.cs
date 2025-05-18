using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core;

public class Comment
{
    [BsonElement("_id")]
    public int CommentId { get; set; }
    public string CommentContent { get; set; }
    public DateTime CommentDate { get; set; }
    public int CommentSubGoalId { get; set; }
    public int StudentId { get; set; }
    public int CommentSenderId { get; set; }
}