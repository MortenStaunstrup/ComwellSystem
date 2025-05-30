using API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Core;

namespace API.Controllers;

[ApiController]
[Route("api/subgoals")]
public class SubGoalController : ControllerBase
{
    private ISubGoalRepository repository;

    public SubGoalController(ISubGoalRepository repo)
    {
        repository = repo;
    }

    [HttpGet]
    public async Task<List<SubGoal>?> GetAllSubGoals()
    {
        Console.WriteLine("Getting all subgoals: controller");
        var result = await repository.GetAllSubGoals();
        // hvis resultatet er null eller en tom liste, returner en tom liste (samme sker i andre funktioner)
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("No subgoals found, returning empty list");
            return new List<SubGoal>();
        }
        return result;
    }
    
    [HttpGet]
    [Route("get/{studentId:int}")]
    public async Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var result = await repository.GetNotCompletedSubGoalsByStudentIdAsync(studentId);
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("No unfinished subgoals for student exist, returning empty list: controller");
            return new List<SubGoal>();
        }
        Console.WriteLine("Returning unfinished subgoals: controller");
        return result;
    }
    
    [HttpGet]
    [Route("getcompleted/{studentId:int}")]
    public async Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var result = await repository.GetCompletedSubGoalsByStudentIdAsync(studentId);
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("No completed subgoals for student exist, returning empty list: controller");
            return new List<SubGoal>();
        }
        Console.WriteLine("Returning completed subgoals: controller");
        return result;
    }
    
    [HttpGet]
    [Route("getstudentextras/{studentId:int}")]
    public async Task<List<SubGoal>?> GetOfferedSubGoalsByStudentIdAsync(int studentId)
    {
        Console.WriteLine($"Trying to get offered subgoals for student {studentId}: controller");
        var result = await repository.GetOfferedSubGoalsByStudentIdAsync(studentId);
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("No offered subgoals for student exist, returning empty list: controller");
            return new List<SubGoal>();
        }
        Console.WriteLine("Returning user offered subgoals: controller");
        return result;
    }
    
    
    [HttpGet]
    [Route("getofferedsubgoals")]
    public async Task<List<SubGoal>?> GetOfferedSubGoalsAsync()
    {
        var result = await repository.GetOfferedSubGoalsAsync();
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("No offered subgoals returning empty list: controller");
            return new List<SubGoal>();
        }
        Console.WriteLine("Returning offered subgoals: controller");
        return result;
    }

    [HttpGet]
    [Route("getpct/{studentId:int}")]
    public async Task<double> GetPctCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var result = await repository.GetPctCompletedSubGoalsByStudentIdAsync(studentId);
        return result;
    }
    
    [HttpPost]
    [Route("create")]
    public void CreateSubgoal(SubGoal subgoal)
    {
        Console.WriteLine("Creating subgoal: controller");
        repository.CreateSubgoal(subgoal);
    }

    [HttpPost]
    [Route("insertall")]
    public void InsertSubgoalAll(SubGoal subgoal)
    {
        Console.WriteLine("Inserting subgoal ALL: controller");
        repository.InsertSubgoalAll(subgoal);
    }

    // SubGoalRequest er en 'DTO' da man kun kan sende ét object over http, og vi gerne vil sende SubGoal og en liste af
    // userId's, laver vi et object til at sende det over http som kun indeholder de 2 ting
    [HttpPost]
    [Route("insertspecific")]
    public void InsertSubgoalSpecific(SubGoalRequest subgoalContainer)
    {
        Console.WriteLine("Inserting subgoal SPECIFIC: controller");
        repository.InsertSubgoalSpecific(subgoalContainer.SubGoal, subgoalContainer.StudentIds);
    }

    [HttpGet]
    [Route("getmaxid")]
    public async Task<int> GetMaxId()
    {
        var result = await repository.MaxSubGoalId();
        Console.WriteLine($"Getting max id {result}: controller");
        return result;
    }
    

    
    [HttpPut]
    [Route("update")]
    public async Task<string> UpdateSubGoalDetails(SubGoal subGoal)
    {
        // try catch prøver at kalde UpdateSubGoalDetails funktion, hvis den fejler fanger den exception og console writer den
        // derefter returnerer den en string "Update failed/successful" afhængig af resultatet
        try
        {
            Console.WriteLine("Updating subgoal: controller");
            await repository.UpdateSubGoalDetails(subGoal);
            return "Update successful";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "Update failed";
        }
    }

    

    [HttpPut]
    [Route("complete/{studentId:int}/{subGoalId:int}")]
    public void CompleteSubGoalBySubGoalId(int studentId, int subGoalId)
    {
        repository.CompleteSubGoalBySubGoalId(subGoalId, studentId);
        Console.WriteLine("Completing Subgoal: controller");
    }

    [HttpDelete]
    [Route("delete/{subGoalId:int}")]
    public void DeleteSubGoalBySubGoalId(int subGoalId)
    {
        repository.DeleteSubGoalBySubGoalId(subGoalId);
        Console.WriteLine("Deleting subgoal: controller");
    }
    
}

