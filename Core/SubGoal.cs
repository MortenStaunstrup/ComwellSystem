namespace Core;

public class SubGoal
{
    private int SubGoalId { get; set; }
    private string SubGoalName { get; set; }
    private string? SubGoalDescription { get; set; }
    private string SubGoalCategory { get; set; }
    private string? SubGoalPicture { get; set; }
    private bool SubGoalStatus { get; set; }
    private DateOnly? SubGoalStartDate { get; set; }
    private DateOnly? SubGoalDueDate { get; set; }
}