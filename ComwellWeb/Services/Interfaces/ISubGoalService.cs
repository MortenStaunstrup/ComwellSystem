using Core;

namespace ComwellWeb.Services.Interfaces;

public interface ISubGoalService
{
    Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId); // Find listen af alle elevens ikke færdiggjorte subgoals, også kaldes 'Student Plan'
    Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId);
    Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync();  // Liste af alle Template subGoals
    
    void CreateSubGoal(SubGoal subGoal, List<int> studentId); // Lav subgoal
    Task<int> MaxSubGoalId(); // Find max id i subgoals
    void InsertSubgoalAll(SubGoal subgoal); // Indsæt subgoal til alle elever
    void InsertSubgoalSpecific(SubGoal subgoal, List<int> studentIds); // Indsæt subgoal til specifikke elever
    void AddSubGoalToTemplates(TemplateSubGoal template); // Lav subgoal og tildel til elev OG tilføj til templates
    
    void UpdateSubGoalDetails(SubGoal subGoal);   // Updater detaljer om subgoal for elev
    void UpdateSubGoalDetailsTemplates(TemplateSubGoal template); // Updater detaljer om subgoal i templates
    void CompleteSubGoalBySubGoalId(int studentId, int subGoalId); // Kryds subgoal af som værende done
    
    void DeleteSubGoalBySubGoalId(int studentId, int subGoalId); // Slet subgoal for elev
    void DeleteTemplateByTemplateId(int templateId); // Slet subgoal i templates
}