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
    public void UpdateSubGoalDetails(SubGoal subGoal)
    {
        Console.WriteLine("Updating subgoal: controller");
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
        Console.WriteLine("Deleting subgoal: controller");
    }

    [HttpDelete]
    [Route("deletetemplate/{templateId:int}")]
    public void DeleteTemplateByTemplateId(int templateId)
    {
        Console.WriteLine("Deleting template: controller");
    }
    
}

public class SubGoalRequest
{
    public SubGoal SubGoal { get; set; }
    public List<int> StudentIds { get; set; }
}