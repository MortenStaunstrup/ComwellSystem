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
        if (result.Count != 0)
        {
            Console.WriteLine("Returning subgoals: controller");
            return result;
        }
        Console.WriteLine("No subgoals for student exist, returning empty list: controller");
        return result;
    }
    
    [HttpGet]
    [Route("gettemplates")]
    public async Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync()
    {
        var result = await repository.GetAllTemplateSubGoalsAsync();
        if (result.Count != 0)
        {
            Console.WriteLine("Returning templates: controller");
            return result;
        }
        Console.WriteLine("No templates exist, returning empty list: controller");
        return result;
    }
    
    [HttpPost]
    [Route("create")]
    public void CreateSubgoal(SubGoal subgoal)
    {
        repository.CreateSubgoal(subgoal);
        Console.WriteLine("Creating subgoal: controller");
    }

    [HttpPost]
    [Route("addtotemplates")]
    public void AddSubGoalToTemplates(TemplateSubGoal template)
    {
        repository.AddSubGoalToTemplates(template);
        Console.WriteLine("Adding template: controller");
    }

    [HttpPut]
    [Route("update")]
    public void UpdateSubGoalDetails(SubGoal subGoal)
    {
        Console.WriteLine("Updating subgoal: controller");
    }

    [HttpPut]
    [Route("updatetemplate")]
    public void UpdateSubGoalDetailsTemplates(TemplateSubGoal template)
    {
        Console.WriteLine("Updating template: controller");
    }

    [HttpPut]
    [Route("complte/{subGoalId:int}")]
    public void CompleteSubGoalBySubGoalId(int subGoalId)
    {
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