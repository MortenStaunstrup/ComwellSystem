using Core;

namespace ComwellWeb.Services.Interfaces;

public interface ISubGoalService
{
    List<SubGoal> GetSubGoalsByStudentIdAsync(int studentId); // Finde listen af alle elevens subgoals, også kaldes 'Student Plan'
    List<TemplateSubGoal> GetAllTemplateSubGoalsAsync();  // Liste af alle Template subGoals
    
    void CreateSubGoal(SubGoal subGoal); // Lav subgoal og tildel til elev
    void AddSubGoalToTemplates(TemplateSubGoal template); // Lav subgoal og tildel til elev OG tilføj til templates
    
    void UpdateSubGoalDetails(SubGoal subGoal);   // Updater detaljer om subgoal for elev
    void UpdateSubGoalDetailsTemplates(TemplateSubGoal template); // Updater detaljer om subgoal i templates
    void CompleteSubGoalBySubGoalId(int subGoalId); // Kryds subgoal af som værende done
    
    void DeleteSubGoalBySubGoalId(int subGoalId); // Slet subgoal for elev
    void DeleteTemplateByTemplateId(int templateId); // Slet subgoal i templates
}