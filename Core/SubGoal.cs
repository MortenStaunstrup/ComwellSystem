using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core;

public class SubGoal
{
    [Required] [BsonElement("_id")] public int SubGoalId { get; set; }
    public Comment[]? Comments { get; set; } = new Comment[0];

    [Required(ErrorMessage = "En opgave skal have et navn")]
    public string SubGoalName { get; set; }

    public string? SubGoalDescription { get; set; }
    
    [Required(ErrorMessage = "En opgave skal have en type")]
    public string SubGoalType { get; set; }

    [Required] public bool SubGoalStatus { get; set; }
    
    public List<MiddleGoal> MiddleGoals { get; set; }

}


// Egen validering, for at sikre at duedate ikke bliver sat før dags dato
public class Deadline : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null && value is DateOnly deadline)
        {
            if(deadline >= DateOnly.FromDateTime(DateTime.Now))
                return ValidationResult.Success;
        }
        return new ValidationResult("Deadline kan ikke være før i dag");
    }
}