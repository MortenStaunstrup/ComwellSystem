using Core;

namespace ComwellWeb.Services.Interfaces;

public interface ISubGoalService
{
    Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId); // Find listen af alle elevens ikke færdiggjorte subgoals, også kaldes 'Student Plan'
    Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId);
    Task<List<SubGoal>?> GetOfferedSubGoalsAsync();
    Task<List<SubGoal>?> GetOfferedSubGoalsByStudentIdAsync(int studentId);
    Task<double> GetPctCompletedSubGoalsByStudentIdAsync(int studentId);
    
    
    void CreateSubGoal(SubGoal subGoal); // Lav subgoal
    Task<int> MaxSubGoalId(); // Find max id i subgoals
    void InsertSubgoalAll(SubGoal subgoal); // Indsæt subgoal til alle elever
    void InsertSubgoalSpecific(SubGoal subgoal, List<int> studentIds); // Indsæt subgoal til specifikke elever
    
    
    void UpdateSubGoalDetails(SubGoal subGoal);   // Updater detaljer om subgoal for elev
    void CompleteSubGoalBySubGoalId(int studentId, int subGoalId); // Kryds subgoal af som værende done
    
    
    void DeleteSubGoalBySubGoalId(int studentId, int subGoalId); // Slet subgoal for elev
}