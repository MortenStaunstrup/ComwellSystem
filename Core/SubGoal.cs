using System.ComponentModel.DataAnnotations;

namespace Core;

public class SubGoal
{
    [Required]
    public int SubGoalId { get; set; }
    [Required(ErrorMessage = "Et Delmål kræver en elev")]
    public int StudentId { get; set; }
    public Comment[]? Comments { get; set; }
    [Required(ErrorMessage = "Et Delmål skal have et navn")]
    public string SubGoalName { get; set; }
    public string? SubGoalDescription { get; set; }
    [Required(ErrorMessage = "Et Delmål skal have en kategori")]
    public string SubGoalCategory { get; set; }
    public string? SubGoalPicture { get; set; }
    [Required]
    public bool SubGoalStatus { get; set; }
    public double? SubGoalTime { get; set; }
    [Required]
    [Deadline]
    public DateOnly SubGoalDueDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    
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