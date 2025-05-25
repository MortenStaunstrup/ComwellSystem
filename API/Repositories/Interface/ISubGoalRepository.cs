using Core;
namespace API.Repositories.Interface;

public interface ISubGoalRepository
{
    
    // Student
    Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId);
    Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId);
    Task<List<SubGoal>?> GetOfferedSubGoalsByStudentIdAsync(int studentId);
    Task<double> GetPctCompletedSubGoalsByStudentIdAsync(int studentId);
    Task<List<SubGoal>?> GetAllSubGoals();
    Task<SubGoal?> GetSubGoalByIdAsync(int id);
    Task<List<SubGoal>?> GetOfferedSubGoalsAsync();
    
    // Create
    void CreateSubgoal(SubGoal subgoal);
    Task<int> MaxSubGoalId();
    void InsertSubgoalAll(SubGoal subgoal);
    void InsertSubgoalSpecific(SubGoal subgoal, List<int> studentIds);
    void UpdateSubGoalDetails(SubGoal subGoal);
    void CompleteSubGoalBySubGoalId(int subGoalId, int studentId);
    void DeleteSubGoalBySubGoalId(int subGoalId);
}