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
            SubGoalCategory = "Køkken kompetence",
            SubGoalDescription = "Du skal lære at snitte en fisk",
            SubGoalDueDate = new DateOnly(2025, 05, 30),
            SubGoalName = "Filering af fisk",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 2,
            Comments = null,
            SubGoalCategory = "Kursus",
            SubGoalDescription = "Learn to bake sourdough bread",
            SubGoalDueDate = new DateOnly(2024, 12, 15),
            SubGoalName = "Sourdough Mastery",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 3,
            Comments = null,
            SubGoalCategory = "Faglig mål",
            SubGoalDescription = "Master the art of sushi making",
            SubGoalDueDate = new DateOnly(2024, 11, 20),
            SubGoalName = "Sushi Rolling",
            SubGoalStatus = true,
        },
        new SubGoal()
        {
            SubGoalId = 4,
            Comments = null,
            SubGoalCategory = "Køkken kompetence",
            SubGoalDescription = "Learn to make croissants from scratch",
            SubGoalDueDate = new DateOnly(2025, 01, 10),
            SubGoalName = "Croissant Perfection",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 5,
            Comments = null,
            SubGoalCategory = "Kursus",
            SubGoalDescription = "Craft the perfect chocolate chip cookies",
            SubGoalDueDate = new DateOnly(2024, 10, 05),
            SubGoalName = "Cookie Crafting",
            SubGoalStatus = true,
        },
        new SubGoal()
        {
            SubGoalId = 6,
            Comments = null,
            SubGoalCategory = "Faglig mål",
            SubGoalDescription = "Make homemade pasta from scratch",
            SubGoalDueDate = new DateOnly(2024, 09, 18),
            SubGoalName = "Pasta Creation",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 7,
            Comments = null,
            SubGoalCategory = "Køkken kompetence",
            SubGoalDescription = "Experiment with plant-based cooking",
            SubGoalDueDate = new DateOnly(2025, 03, 12),
            SubGoalName = "Vegan Ventures",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 8,
            Comments = null,
            SubGoalCategory = "Kursus",
            SubGoalDescription = "Plan and execute a 3-course meal",
            SubGoalDueDate = new DateOnly(2024, 08, 22),
            SubGoalName = "Three-Course Challenge",
            SubGoalStatus = true,
        },
        new SubGoal()
        {
            SubGoalId = 9,
            Comments = null,
            SubGoalCategory = "Faglig mål",
            SubGoalDescription = "Learn to make kimchi",
            SubGoalDueDate = new DateOnly(2025, 04, 05),
            SubGoalName = "Kimchi Craft",
            SubGoalStatus = false,
        },
        new SubGoal()
        {
            SubGoalId = 10,
            Comments = null,
            SubGoalCategory = "Køkken kompetence",
            SubGoalDescription = "Pair wines with different dishes",
            SubGoalDueDate = new DateOnly(2024, 07, 30),
            SubGoalName = "Wine Pairing",
            SubGoalStatus = false,
        }
    };

    private List<TemplateSubGoal> templates = new List<TemplateSubGoal>()
    {
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 1,
            TemplateSubGoalName = "Sushi Rolling",
            TemplateSubGoalCategory = "Kursus",
            TemplateSubGoalDescription = "Exciting new ways to roll sushi for the best taste"
        },
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 2,
            TemplateSubGoalName = "Bread Baking Basics",
            TemplateSubGoalCategory = "Faglig mål",
            TemplateSubGoalDescription = "Learn the foundations of bread baking"
        },
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 3,
            TemplateSubGoalName = "Vegetarian Cooking",
            TemplateSubGoalCategory = "Køkken kompetence",
            TemplateSubGoalDescription = "Explore creative vegetarian recipes"
        },
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 4,
            TemplateSubGoalName = "Advanced Pastry Techniques",
            TemplateSubGoalCategory = "Kursus",
            TemplateSubGoalDescription = "Master the art of pastry with advanced techniques"
        },
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 5,
            TemplateSubGoalName = "Gourmet Sauces",
            TemplateSubGoalCategory = "Faglig mål",
            TemplateSubGoalDescription = "Create gourmet sauces from scratch"
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

    public async Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync()
    {
        return templates;
    }

    public void CreateSubGoal(SubGoal subGoal)
    {
        subGoal.SubGoalId = subGoals.Max(s => s.SubGoalId) + 1;
        subGoals.Add(subGoal);
        Console.WriteLine("Adding subgoal to student in services");
    }

    public void AddSubGoalToTemplates(TemplateSubGoal template)
    {
        template.TemplateSubGoalId = templates.Max(t => t.TemplateSubGoalId) + 1;
        templates.Add(template);
        Console.WriteLine("Adding subgoal to templates in services");
    }

    public void UpdateSubGoalDetails(SubGoal subGoal)
    {
        int index = subGoals.FindIndex(x => x.SubGoalId == subGoal.SubGoalId);
        subGoals.RemoveAt(index);
        subGoals.Insert(index, subGoal);
        Console.WriteLine("Updating subgoal in services");
    }

    public void UpdateSubGoalDetailsTemplates(TemplateSubGoal template)
    {
        int index = templates.FindIndex(x => x.TemplateSubGoalId == template.TemplateSubGoalId);
        templates.RemoveAt(index);
        templates.Insert(index, template);
        Console.WriteLine("Updating template in services");
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

    public void DeleteTemplateByTemplateId(int templateId)
    {
        templates.RemoveAll(x => x.TemplateSubGoalId == templateId);
        Console.WriteLine("Deleting template in services");
    }
}