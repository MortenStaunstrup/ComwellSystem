using System.Net;
using System.Net.Http.Json;
using ComwellWeb.Services.Interfaces;
using Core;

namespace ComwellWeb.Services;

public class SubGoalServiceServer : ISubGoalService
{
    private HttpClient _client;
    private readonly string BaseURL = "http://localhost:5116/api/subgoals";

    public SubGoalServiceServer(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<List<SubGoal>?> GetSubGoalsByStudentIdAsync(int studentId)
    {
        return await _client.GetFromJsonAsync<List<SubGoal>?>($"{BaseURL}/subgoals/get/{studentId}");
    }

    public async Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync()
    {
        return await _client.GetFromJsonAsync<List<TemplateSubGoal>?>($"{BaseURL}/gettemplates");
    }

    public void CreateSubGoal(SubGoal subGoal)
    {
        
    }

    public void AddSubGoalToTemplates(TemplateSubGoal template)
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