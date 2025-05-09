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
            StudentId = 101,
            Comments = null,
            SubGoalCategory = "Køkken kompetence",
            SubGoalDescription = "Du skal lære at snitte en fisk",
            SubGoalDueDate = new DateOnly(2025, 05, 30),
            SubGoalName = "Filering af fisk",
            SubGoalPicture = null,
            SubGoalStatus = false,
            SubGoalTime = 7.5
        },
        new SubGoal()
        {
            SubGoalId = 2,
            StudentId = 102,
            Comments = null,
            SubGoalCategory = "Kursus",
            SubGoalDescription = "Learn to bake sourdough bread",
            SubGoalDueDate = new DateOnly(2024, 12, 15),
            SubGoalName = "Sourdough Mastery",
            SubGoalPicture = null,
            SubGoalStatus = false,
            SubGoalTime = 5.0
        },
        new SubGoal()
        {
            SubGoalId = 3,
            StudentId = 103,
            Comments = null,
            SubGoalCategory = "Faglig mål",
            SubGoalDescription = "Master the art of sushi making",
            SubGoalDueDate = new DateOnly(2024, 11, 20),
            SubGoalName = "Sushi Rolling",
            SubGoalPicture = null,
            SubGoalStatus = true,
            SubGoalTime = 10.0
        },
        new SubGoal()
        {
            SubGoalId = 4,
            StudentId = 104,
            Comments = null,
            SubGoalCategory = "Køkken kompetence",
            SubGoalDescription = "Learn to make croissants from scratch",
            SubGoalDueDate = new DateOnly(2025, 01, 10),
            SubGoalName = "Croissant Perfection",
            SubGoalPicture = null,
            SubGoalStatus = false,
            SubGoalTime = 8.0
        },
        new SubGoal()
        {
            SubGoalId = 5,
            StudentId = 105,
            Comments = null,
            SubGoalCategory = "Kursus",
            SubGoalDescription = "Craft the perfect chocolate chip cookies",
            SubGoalDueDate = new DateOnly(2024, 10, 05),
            SubGoalName = "Cookie Crafting",
            SubGoalPicture = null,
            SubGoalStatus = true,
            SubGoalTime = 3.0
        },
        new SubGoal()
        {
            SubGoalId = 6,
            StudentId = 106,
            Comments = null,
            SubGoalCategory = "Faglig mål",
            SubGoalDescription = "Make homemade pasta from scratch",
            SubGoalDueDate = new DateOnly(2024, 09, 18),
            SubGoalName = "Pasta Creation",
            SubGoalPicture = null,
            SubGoalStatus = false,
            SubGoalTime = 6.5
        },
        new SubGoal()
        {
            SubGoalId = 7,
            StudentId = 107,
            Comments = null,
            SubGoalCategory = "Køkken kompetence",
            SubGoalDescription = "Experiment with plant-based cooking",
            SubGoalDueDate = new DateOnly(2025, 03, 12),
            SubGoalName = "Vegan Ventures",
            SubGoalPicture = null,
            SubGoalStatus = false,
            SubGoalTime = 4.0
        },
        new SubGoal()
        {
            SubGoalId = 8,
            StudentId = 108,
            Comments = null,
            SubGoalCategory = "Kursus",
            SubGoalDescription = "Plan and execute a 3-course meal",
            SubGoalDueDate = new DateOnly(2024, 08, 22),
            SubGoalName = "Three-Course Challenge",
            SubGoalPicture = null,
            SubGoalStatus = true,
            SubGoalTime = 12.0
        },
        new SubGoal()
        {
            SubGoalId = 9,
            StudentId = 109,
            Comments = null,
            SubGoalCategory = "Faglig mål",
            SubGoalDescription = "Learn to make kimchi",
            SubGoalDueDate = new DateOnly(2025, 04, 05),
            SubGoalName = "Kimchi Craft",
            SubGoalPicture = null,
            SubGoalStatus = false,
            SubGoalTime = 2.0
        },
        new SubGoal()
        {
            SubGoalId = 10,
            StudentId = 110,
            Comments = null,
            SubGoalCategory = "Køkken kompetence",
            SubGoalDescription = "Pair wines with different dishes",
            SubGoalDueDate = new DateOnly(2024, 07, 30),
            SubGoalName = "Wine Pairing",
            SubGoalPicture = null,
            SubGoalStatus = false,
            SubGoalTime = 9.0
        }
    };

    private List<TemplateSubGoal> templates = new List<TemplateSubGoal>()
    {
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 1,
            TemplateSubGoalName = "Sushi Rolling",
            TemplateSubGoalPicture = null,
            TemplateSubGoalCategory = "Kursus",
            TemplateSubGoalDescription = "Exciting new ways to roll sushi for the best taste"
        },
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 2,
            TemplateSubGoalName = "Bread Baking Basics",
            TemplateSubGoalPicture = null,
            TemplateSubGoalCategory = "Faglig mål",
            TemplateSubGoalDescription = "Learn the foundations of bread baking"
        },
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 3,
            TemplateSubGoalName = "Vegetarian Cooking",
            TemplateSubGoalPicture = null,
            TemplateSubGoalCategory = "Køkken kompetence",
            TemplateSubGoalDescription = "Explore creative vegetarian recipes"
        },
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 4,
            TemplateSubGoalName = "Advanced Pastry Techniques",
            TemplateSubGoalPicture = null,
            TemplateSubGoalCategory = "Kursus",
            TemplateSubGoalDescription = "Master the art of pastry with advanced techniques"
        },
        new TemplateSubGoal()
        {
            TemplateSubGoalId = 5,
            TemplateSubGoalName = "Gourmet Sauces",
            TemplateSubGoalPicture = null,
            TemplateSubGoalCategory = "Faglig mål",
            TemplateSubGoalDescription = "Create gourmet sauces from scratch"
        }
    };


    public List<SubGoal> GetSubGoalsByStudentIdAsync(int studentId)
    {
        return subGoals.FindAll(x => x.StudentId == studentId);
    }

    public List<TemplateSubGoal> GetAllTemplateSubGoalsAsync()
    {
        return templates;
    }

    public void CreateSubGoal(SubGoal subGoal)
    {
        subGoal.SubGoalId = subGoals.Max(s => s.SubGoalId) + 1;
        subGoals.Add(subGoal);
    }

    public void AddSubGoalToTemplates(TemplateSubGoal template)
    {
        template.TemplateSubGoalId = templates.Max(t => t.TemplateSubGoalId) + 1;
        templates.Add(template);
    }

    public void UpdateSubGoalDetails(SubGoal subGoal)
    {
        int index = subGoals.FindIndex(x => x.SubGoalId == subGoal.SubGoalId);
        subGoals.RemoveAt(index);
        subGoals.Insert(index, subGoal);
    }

    public void UpdateSubGoalDetailsTemplates(TemplateSubGoal template)
    {
        int index = templates.FindIndex(x => x.TemplateSubGoalId == template.TemplateSubGoalId);
        templates.RemoveAt(index);
        templates.Insert(index, template);
    }

    public void CompleteSubGoalBySubGoalId(int subGoalId)
    {
        subGoals.Find(x => x.SubGoalId == subGoalId).SubGoalStatus = true;
    }

    public void DeleteSubGoalBySubGoalId(int subGoalId)
    {
        subGoals.RemoveAll(x => x.SubGoalId == subGoalId);
    }

    public void DeleteTemplateByTemplateId(int templateId)
    {
        templates.RemoveAll(x => x.TemplateSubGoalId == templateId);
    }
}