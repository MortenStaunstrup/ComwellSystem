namespace Core;

public class SubGoal
{
    public int SubGoalId { get; set; }
    public string SubGoalName { get; set; }
    public string? SubGoalDescription { get; set; }
    public string SubGoalCategory { get; set; }
    public string? SubGoalPicture { get; set; }
    public bool SubGoalStatus { get; set; }
    public DateOnly? SubGoalStartDate { get; set; }
    public DateOnly? SubGoalDueDate { get; set; }
}