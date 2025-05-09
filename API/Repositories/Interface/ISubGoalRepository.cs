using Core;
namespace API.Repositories.Interface;

public interface ISubGoalRepository
{
    Task<List<SubGoal>?> GetSubGoalsByStudentIdAsync(int studentId);
    Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync();
    void CreateSubgoal();
    void AddSubGoalToTemplates();
    void UpdateSubGoalDetails(SubGoal subGoal);
    void UpdateSubGoalDetailsTemplates(TemplateSubGoal template);
    void CompleteSubGoalBySubGoalId(int subGoalId);
    void DeleteSubGoalBySubGoalId(int subGoalId);
    void DeleteTemplateByTemplateId(int templateId);
}