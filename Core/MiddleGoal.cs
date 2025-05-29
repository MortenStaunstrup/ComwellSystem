namespace Core;

public class MiddleGoal
{
    public List<MiniGoal>? MiniGoals { get; set; } // indeholder de minigoals, som er embedded i middlegoal
    public string Name { get; set; } // PK
    public bool Status { get; set; } // hvordan ved vi, om middlegoal er færdigt?
}