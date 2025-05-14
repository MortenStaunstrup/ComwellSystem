using Core;
namespace API.Repositories.Interface;

public interface ISubGoalRepository
{
    Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId);
    Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId);
    Task<SubGoal?> GetSubGoalByIdAsync(int id);
    Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync();
    void CreateSubgoal(SubGoal subgoal, List<int> studentIds);
    Task<int> MaxSubGoalId();
    void InsertSubgoalAll(SubGoal subgoal);
    void InsertSubgoalSpecific(SubGoal subgoal, List<int> studentIds);
    void AddSubGoalToTemplates(TemplateSubGoal template);
    void UpdateSubGoalDetails(SubGoal subGoal);
    void UpdateSubGoalDetailsTemplates(TemplateSubGoal template);
    void CompleteSubGoalBySubGoalId(int subGoalId, int studentId);
    void DeleteSubGoalBySubGoalId(int subGoalId, int studentId);
    void DeleteTemplateByTemplateId(int templateId);
}