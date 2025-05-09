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
    public async Task<List<SubGoal>?> GetSubGoalsByStudentIdAsync(int studentId)
    {
        var result = await repository.GetSubGoalsByStudentIdAsync(studentId);
        if (result != null)
        {
            Console.WriteLine("Returning subgoals from controller");
            return result;
        }
        Console.WriteLine("No subgoals for student exist");
        return null;
    }
    
    [HttpGet]
    [Route("gettemplates")]
    public async Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync()
    {
        var result = await repository.GetAllTemplateSubGoalsAsync();
        if (result != null)
        {
            Console.WriteLine("Returning templates");
            return result;
        }
        Console.WriteLine("No templates exist");
        return null;
    }
    
    [HttpPost]
    [Route("create")]
    public void CreateSubgoal()
    {
        Console.WriteLine("Creating subgoal in controller");
    }

    [HttpPost]
    [Route("addtotemplates")]
    public void AddSubGoalToTemplates()
    {
        Console.WriteLine("Adding to template in controller");
    }

    [HttpPut]
    [Route("update")]
    public void UpdateSubGoalDetails(SubGoal subGoal)
    {
        Console.WriteLine("Updating subgoal in controller");
    }

    [HttpPut]
    [Route("updatetemplate")]
    public void UpdateSubGoalDetailsTemplates(TemplateSubGoal template)
    {
        Console.WriteLine("Updating template in controller");
    }

    [HttpPut]
    [Route("complte/{subGoalId:int}")]
    public void CompleteSubGoalBySubGoalId(int subGoalId)
    {
        Console.WriteLine("Completing Subgoal in controller");
    }

    [HttpDelete]
    [Route("delete/{subGoalId:int}")]
    public void DeleteSubGoalBySubGoalId(int subGoalId)
    {
        Console.WriteLine("Deleting subgoal in controller");
    }

    [HttpDelete]
    [Route("deletetemplate/{templateId:int}")]
    public void DeleteTemplateByTemplateId(int templateId)
    {
        Console.WriteLine("Deleting template in controller");
    }
    
}