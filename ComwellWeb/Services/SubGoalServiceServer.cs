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
    
    public async Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        Console.WriteLine($"Getting student {studentId} unfinished subgoals: service");
        var result = await _client.GetFromJsonAsync<List<SubGoal>?>($"{BaseURL}/get/{studentId}");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Subgoals either null or empty list, returning null: service");
            return null;
        }

        return result;
    }

    public async Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        Console.WriteLine($"Getting finished student {studentId} finished subgoals: service");
        var result = await _client.GetFromJsonAsync<List<SubGoal>?>($"{BaseURL}/getcompleted/{studentId}");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Subgoals either null or empty list, returning null: service");
            return null;
        }

        return result;
    }
    

    public async Task<List<SubGoal>?> GetOfferedSubGoalsAsync()
    {
        Console.WriteLine("Getting offered subgoals: service");
        var result = await _client.GetFromJsonAsync<List<SubGoal>?>($"{BaseURL}/getofferedsubgoals");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Offered subgoals either null or empty list, returning null: service");
            return null;
        }
        Console.WriteLine("Returning Offered subgoals: service");
        return result;
    }
    
    public async void CreateSubGoal(SubGoal subGoal, List<int> studentId)
    {
        Console.WriteLine("Creating subgoal: service");
        var request = new SubGoalRequest()
        {
            SubGoal = subGoal,
            StudentIds = studentId
        };
        await _client.PostAsJsonAsync($"{BaseURL}/create", request);
    }

    public async Task<int> MaxSubGoalId()
    {
        Console.WriteLine("Getting max subgoalsId: service");
        return await _client.GetFromJsonAsync<int>($"{BaseURL}/getmaxid");
    }

    public async void InsertSubgoalAll(SubGoal subgoal)
    {
        Console.WriteLine("Inserting subgoal ALL: service");
        await _client.PostAsJsonAsync($"{BaseURL}/insertall", subgoal);
    }

    public async void InsertSubgoalSpecific(SubGoal subgoal, List<int> studentIds)
    {
        var request = new SubGoalRequest()
        {
            StudentIds = studentIds,
            SubGoal = subgoal
        };
        Console.WriteLine("Inserting subgoal SPECIFIC: service");
        await _client.PostAsJsonAsync($"{BaseURL}/insertspecific", request);
    }
    

    public void UpdateSubGoalDetails(SubGoal subGoal)
    {
        
    }

    public async void CompleteSubGoalBySubGoalId(int studentId, int subGoalId)
    {
        Console.WriteLine($"Completing subgoal {subGoalId} for student {studentId}");
        await _client.PostAsJsonAsync($"{BaseURL}/complete/{studentId}/{subGoalId}", true);
    }

    public void DeleteSubGoalBySubGoalId(int studentId, int subGoalId)
    {
        
    }

    public void DeleteTemplateByTemplateId(int templateId)
    {
        
    }
}

public class SubGoalRequest
{
    public SubGoal SubGoal { get; set; }
    public List<int> StudentIds { get; set; }
}