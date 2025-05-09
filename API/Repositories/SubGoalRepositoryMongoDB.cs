using API.Repositories.Interface;
using Core;

namespace API.Repositories;

public class SubGoalRepositoryMongoDB : ISubGoalRepository
{
    private readonly string _connectionString;

    public SubGoalRepositoryMongoDB()
    {
        _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("No connection string configured");
        }
    }
    
    public async Task<List<SubGoal>?> GetSubGoalsByStudentIdAsync(int studentId)
    {

    }

    public async Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync()
    {

    }

    public void CreateSubgoal()
    {

    }

    public void AddSubGoalToTemplates()
    {

    }

    public void UpdateSubGoalDetails(SubGoal subGoal)
    {

    }

    public void UpdateSubGoalDetailsTemplates(TemplateSubGoal template)
    {
        
    }

    public void CompleteSubGoalBySubGoalId(int subGoalId)
    {
        
    }

    public void DeleteSubGoalBySubGoalId(int subGoalId)
    {
        
    }

    public void DeleteTemplateByTemplateId(int templateId)
    {
        
    }
}