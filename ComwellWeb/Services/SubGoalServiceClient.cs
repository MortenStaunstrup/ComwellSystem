using ComwellWeb.Services.Interfaces;
using Core;

namespace ComwellWeb.Services;

public class SubGoalServiceClient : ISubGoalService
{
    private List<SubGoal> subGoals = new List<SubGoal>()
    {
        new SubGoal()
        {
            SubGoalId = 1,
            Comments = null,
            SubGoalDescription = "Du skal lære at snitte en fisk",
            SubGoalName = "Filering af fisk",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 2,
            Comments = null,
            SubGoalDescription = "Learn to bake sourdough bread",
            SubGoalName = "Sourdough Mastery",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 3,
            Comments = null,
            SubGoalDescription = "Master the art of sushi making",
            SubGoalName = "Sushi Rolling",
            SubGoalStatus = true,
        },
        new SubGoal()
        {
            SubGoalId = 4,
            Comments = null,
            SubGoalDescription = "Learn to make croissants from scratch",
            SubGoalName = "Croissant Perfection",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 5,
            Comments = null,
            SubGoalDescription = "Craft the perfect chocolate chip cookies",
            SubGoalName = "Cookie Crafting",
            SubGoalStatus = true,
        },
        new SubGoal()
        {
            SubGoalId = 6,
            Comments = null,
            SubGoalDescription = "Make homemade pasta from scratch",
            SubGoalName = "Pasta Creation",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 7,
            Comments = null,
            SubGoalDescription = "Experiment with plant-based cooking",
            SubGoalName = "Vegan Ventures",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 8,
            Comments = null,
            SubGoalDescription = "Plan and execute a 3-course meal",
            SubGoalName = "Three-Course Challenge",
            SubGoalStatus = true,
        },
        new SubGoal()
        {
            SubGoalId = 9,
            Comments = null,
            SubGoalDescription = "Learn to make kimchi",
            SubGoalName = "Kimchi Craft",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 10,
            Comments = null,
            SubGoalDescription = "Pair wines with different dishes",
            SubGoalName = "Wine Pairing",
            SubGoalStatus = false,
        }
    };
    


    public async Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        return null;
    }
    
    public async Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        return null;
    }
    

    public Task<List<SubGoal>?> GetOfferedSubGoalsAsync()
    {
        return null;
    }

    public async Task<double> GetPctCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        return 0;
    }

    public async Task<List<SubGoal>?> GetOfferedSubGoalsByStudentIdAsync(int studentId)
    {
        return null;
    }
    
    public void CreateSubGoal(SubGoal subGoal)
    {
        subGoal.SubGoalId = subGoals.Max(s => s.SubGoalId) + 1;
        subGoals.Add(subGoal);
        Console.WriteLine("Adding subgoal to student in services");
    }
    

    public void UpdateSubGoalDetails(SubGoal subGoal)
    {
        int index = subGoals.FindIndex(x => x.SubGoalId == subGoal.SubGoalId);
        subGoals.RemoveAt(index);
        subGoals.Insert(index, subGoal);
        Console.WriteLine("Updating subgoal in services");
    }
    

    public void CompleteSubGoalBySubGoalId(int studentId, int subGoalId)
    {
        subGoals.Find(x => x.SubGoalId == subGoalId).SubGoalStatus = true;
        Console.WriteLine("Completing subgoal in services");
    }

    public void DeleteSubGoalBySubGoalId(int studentId, int subGoalId)
    {
        subGoals.RemoveAll(x => x.SubGoalId == subGoalId);
        Console.WriteLine("Deleting subgoal in services");
    }
    
    
    public void InsertSubgoalAll(SubGoal subgoal)
    {
        throw new NotImplementedException();
    }

    public void InsertSubgoalSpecific(SubGoal subgoal, List<int> studentIds)
    {
        throw new NotImplementedException();
    }

    public Task<int> MaxSubGoalId()
    {
        throw new NotImplementedException();
    }
}