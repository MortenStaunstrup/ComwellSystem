using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core;

public class TemplateSubGoal
{
    [BsonElement("_id")]
    public int TemplateSubGoalId { get; set; }
    public string TemplateSubGoalName { get; set; }
    public string TemplateSubGoalDescription { get; set; }
    public string TemplateSubGoalCategory { get; set; }
}