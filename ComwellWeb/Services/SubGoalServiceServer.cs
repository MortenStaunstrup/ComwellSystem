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
        Console.WriteLine($"Getting student {studentId} subgoals from service");
        var result = await _client.GetFromJsonAsync<List<SubGoal>?>($"{BaseURL}/subgoals/get/{studentId}");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Subgoals either null or empty list, returning null: service");
            return null;
        }

        return result;
    }

    public async Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync()
    {
        Console.WriteLine("Getting templates: service");
        var result = await _client.GetFromJsonAsync<List<TemplateSubGoal>?>($"{BaseURL}/gettemplates");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Templates either null or empty list, returning null: service");
            return null;
        }
        Console.WriteLine("Returning templates: service");
        return result;
    }

    public void CreateSubGoal(SubGoal subGoal)
    {
        Console.WriteLine("Creating subgoal: service");
        _client.PostAsJsonAsync($"{BaseURL}/create", subGoal);
    }

    public void AddSubGoalToTemplates(TemplateSubGoal template)
    {
        Console.WriteLine("Creating template: service");
        _client.PostAsJsonAsync($"{BaseURL}/addtotemplates", template);
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