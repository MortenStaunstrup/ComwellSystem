using Core;
namespace API.Repositories.Interface;

public interface ISubGoalRepository
{
    Task<List<SubGoal>?> GetSubGoalsByStudentIdAsync(int studentId);
    Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync();
    void CreateSubgoal(SubGoal subgoal);
    void AddSubGoalToTemplates(TemplateSubGoal template);
    void UpdateSubGoalDetails(SubGoal subGoal);
    void UpdateSubGoalDetailsTemplates(TemplateSubGoal template);
    void CompleteSubGoalBySubGoalId(int subGoalId, int studentId);
    void DeleteSubGoalBySubGoalId(int subGoalId, int studentId);
    void DeleteTemplateByTemplateId(int templateId);
}