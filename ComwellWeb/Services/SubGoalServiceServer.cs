using System.Net;
using System.Net.Http.Json;
using ComwellWeb.Services.Interfaces;
using Core;

namespace ComwellWeb.Services;

public class SubGoalServiceServer : ISubGoalService
{
    private HttpClient _client;

    public SubGoalServiceServer(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<SubGoal>?> GetAllSubGoals()
    {
        Console.WriteLine("Getting all subgoals: service");
        var result = await _client.GetFromJsonAsync<List<SubGoal>?>("subgoals");
        return result;
    }
    
    public async Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        Console.WriteLine($"Getting student {studentId} unfinished subgoals: service");
        var result = await _client.GetFromJsonAsync<List<SubGoal>?>($"subgoals/get/{studentId}");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Subgoals either null or empty list, returning null: service");
            return null;
        }

        return result;
    }

    public async Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        Console.WriteLine($"Getting student {studentId} finished subgoals: service");
        var result = await _client.GetFromJsonAsync<List<SubGoal>?>($"subgoals/getcompleted/{studentId}");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Subgoals either null or empty list, returning null: service");
            return null;
        }

        return result;
    }

    // Returnerer en elevs 'Extra' SubGoals
    public async Task<List<SubGoal>?> GetOfferedSubGoalsByStudentIdAsync(int studentId)
    {
        Console.WriteLine($"Getting student {studentId} offered subgoals: service");
        var result = await _client.GetFromJsonAsync<List<SubGoal>?>($"subgoals/getstudentextras/{studentId}");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Subgoals either null or empty list, returning null: service");
            return null;
        }
        Console.WriteLine($"Returning offered subgoals for student {studentId}: service");
        return result;
    }

    // Returnerer procentdel Completed subgoals ud fra total mængde af subgoals en elev har
    public async Task<double> GetPctCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var result = await _client.GetFromJsonAsync<double>($"subgoals/getpct/{studentId}");
        return result;
    }
    

    // Finder alle 'extra' subgoals i systemet
    public async Task<List<SubGoal>?> GetOfferedSubGoalsAsync()
    {
        Console.WriteLine("Getting offered subgoals: service");
        var result = await _client.GetFromJsonAsync<List<SubGoal>?>($"subgoals/getofferedsubgoals");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Offered subgoals either null or empty list, returning null: service");
            return null;
        }
        Console.WriteLine("Returning Offered subgoals: service");
        return result;
    }
    
    public async void CreateSubGoal(SubGoal subGoal)
    {
        Console.WriteLine("Creating subgoal: service");
        await _client.PostAsJsonAsync($"subgoals/create", subGoal);
    }

    public async Task<int> MaxSubGoalId()
    {
        Console.WriteLine("Getting max subgoalsId: service");
        return await _client.GetFromJsonAsync<int>($"subgoals/getmaxid");
    }

    // Indsætter subgoal ind i alle elever (kun for 'standard' type subgoals
    public async void InsertSubgoalAll(SubGoal subgoal)
    {
        Console.WriteLine("Inserting subgoal ALL: service");
        await _client.PostAsJsonAsync($"subgoals/insertall", subgoal);
    }

    // Laver en 'DTO' så vi kan sende 2 objecter i ét object
    public async void InsertSubgoalSpecific(SubGoal subgoal, List<int> studentIds)
    {
        var request = new SubGoalRequest()
        {
            StudentIds = studentIds,
            SubGoal = subgoal
        };
        Console.WriteLine("Inserting subgoal SPECIFIC: service");
        await _client.PostAsJsonAsync($"subgoals/insertspecific", request);
    }
    

    public async Task<SubGoal> UpdateSubGoalDetails(SubGoal subGoal)
    {
        Console.WriteLine("Updating subgoal: service");

        var response = await _client.PutAsJsonAsync("subgoals/update", subGoal);

        // Hvis statuskoden er 200, returner subgoal, hvis ikke returner null
        if (response.IsSuccessStatusCode)
        {
            var updatedSubGoal = await response.Content.ReadFromJsonAsync<SubGoal>();
            return updatedSubGoal;
        }
        else
        {
            Console.WriteLine($"Update failed with status: {response.StatusCode}");
            return null;
        }
    }


    public async void CompleteSubGoalBySubGoalId(int studentId, int subGoalId)
    {
        Console.WriteLine($"Completing subgoal {subGoalId} for student {studentId}");
        await _client.PostAsJsonAsync($"subgoals/complete/{studentId}/{subGoalId}", true);
    }

    public async void DeleteSubGoalBySubGoalId(int subGoalId)
    {
        Console.WriteLine($"Deleting subgoal {subGoalId}: service"); 
        await _client.DeleteAsync($"subgoals/delete/{subGoalId}");
    }
    
}