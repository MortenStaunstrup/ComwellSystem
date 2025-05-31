using System.Collections.Immutable;
using API.Repositories.Interface;
using Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace API.Repositories;

public class SubGoalRepositoryMongoDB : ISubGoalRepository
{
    private readonly string _connectionString;
    private IMongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<SubGoal> subCollection;
    private IMongoCollection<User> userCollection;
    private IMongoCollection<Notification> notiCollection;
    private GridFSBucket bucket;

    public SubGoalRepositoryMongoDB()
    {
        // connectionstring initialiseres, kunne være med DotEnv fil
        _connectionString = "mongodb+srv://mortenstnielsen:hlEgCKJrN89edDQt@clusterfree.a2y2b.mongodb.net/";
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("No connection string configured");
        }
        client = new MongoClient(_connectionString);
        database = client.GetDatabase("Comwell");
        subCollection = database.GetCollection<SubGoal>("SubGoals");
        userCollection = database.GetCollection<User>("Users");
        notiCollection = database.GetCollection<Notification>("Notifications");
        bucket = new GridFSBucket(database, new GridFSBucketOptions{ BucketName = "Comwell files" });
    }
    
    //mener at denne funktion ikke bliver brugt længere
    public async Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var studentFilter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
        var projection = Builders<User>.Projection.Include("StudentPlan").Exclude("_id");

        var user = await userCollection
            .Find(studentFilter)
            .Project<User>(projection)
            .FirstOrDefaultAsync();

        if (user?.StudentPlan == null)
        {
            return null;
        }

        var subgoals = user.StudentPlan
            .Where(subgoal => subgoal.SubGoalStatus == false)
            .ToList();
        
        
        Console.WriteLine($"Returning unfinished subgoals for student {studentId}");
        return subgoals;
    }

    //mener at denne funktion ikke bliver brugt længere
    public async Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var studentFilter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
        var projection = Builders<User>.Projection.Include("StudentPlan").Exclude("_id");

        var user = await userCollection
            .Find(studentFilter)
            .Project<User>(projection)
            .FirstOrDefaultAsync();

        if (user?.StudentPlan == null)
        {
            return null;
        }

        var subgoals = user.StudentPlan
            .Where(subgoal => subgoal.SubGoalStatus == true)
            .ToList();
        
        Console.WriteLine($"Returning finished subgoals for student {studentId}");
        return subgoals;
    }

    public async Task<List<SubGoal>?> GetAllSubGoals()
    {
        var filter = Builders<SubGoal>.Filter.Empty;
        return await subCollection.Find(filter).ToListAsync();
    }

    public async Task<double> GetPctCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var studentFilter = Builders<User>.Filter.Eq("_id", studentId);

        // 'Unwind' laver et dokument af hvert element i arrayet
        var aggregation = await userCollection.Aggregate()
            .Match(studentFilter)
            .Unwind(u => u.StudentPlan)
            // Gruper dem efter SubGoalStatus og ét 1 tal
            .Group(new BsonDocument
            {
                { "_id", "$StudentPlan.SubGoalStatus" },
                { "Count", new BsonDocument("$sum", 1) }
            })
            .ToListAsync();
        
        int totalCount = 0;
        int completedCount = 0;

        // For hvert resultat i aggregeringen, tæl total værdien op.
        // Hvis bool er sandt, tæl completed værdien op
        foreach (var result in aggregation)
        {
            bool status = result["_id"].AsBoolean;
            int count = result["Count"].AsInt32;
            totalCount += count;
            if (status)
            {
                completedCount += count;
            }
        }

        if (totalCount == 0)
        {
            return 0;
        }

        // udregn procenten
        var percentage = (completedCount / (double)totalCount) * 100;
        Console.WriteLine($"The result for student {studentId} is {percentage}");
        return percentage;
    }

    // Finder alle 'Extra' opgaver som en elev har
    // Bliver brug til at sørge for, at elever ikke kan byde på 'Extra' opgaver de allerede har på 'RegisterSubgoal' pagen
    public async Task<List<SubGoal>?> GetOfferedSubGoalsByStudentIdAsync(int studentId)
    {
        var studentFilter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
        var subGoalFilter = Builders<User>.Filter.ElemMatch(x => x.StudentPlan, s => s.SubGoalType == "Extra");
        var combinedFilter = Builders<User>.Filter.And(studentFilter, subGoalFilter);
        // projecterer kun StudentPlan feltet fra User modelklassen
        var projection = Builders<User>.Projection.Include("StudentPlan").Exclude("_id");
        
        var user = await userCollection
            .Find(combinedFilter)
            .Project<User>(projection)
            .FirstOrDefaultAsync();

        if (user?.StudentPlan == null)
            return null;
        
        Console.WriteLine($"Returning extra subgoals for student {studentId}");
        return user.StudentPlan;
    }

    // Find ALLE extra delmål fra SubGoals collection
    // Bruges til at vise alle de 'Extra' delmål som en elev kan byde ind på
    public async Task<List<SubGoal>?> GetOfferedSubGoalsAsync()
    {
        var filter = Builders<SubGoal>.Filter.Eq("SubGoalType", "Extra");
        var result = await subCollection.Find(filter).ToListAsync();
        
        return result;
    }

    public async Task<SubGoal?> GetSubGoalByIdAsync(int id)
    {
        var filter = Builders<SubGoal>.Filter.Eq(x => x.SubGoalId, id);
        var subGoal = await subCollection.Find(filter).FirstOrDefaultAsync();
        if (subGoal == null)
            return null;
        return subGoal;
    }

    public async Task<int> MaxSubGoalId()
    {
        var sort = Builders<SubGoal>.Sort.Descending(x => x.SubGoalId);
        var subGoal = await subCollection
            .Find(Builders<SubGoal>.Filter.Empty)
            .Sort(sort)
            .Limit(1)
            .FirstOrDefaultAsync();
        return subGoal?.SubGoalId ?? 0;
    }

    public async void CreateSubgoal(SubGoal subgoal)
    {
        // Indsætter subgoal i SubGoal collection
        await subCollection.InsertOneAsync(subgoal);
        Console.WriteLine($"Inserting subgoal {subgoal.SubGoalId} into subgoal collection");
    }


    // Indsætter subgoal i alle elever (type = "Standard")
    public async void InsertSubgoalAll(SubGoal subgoal)
    {
        var filter = Builders<User>.Filter.Eq("Role", "Student");
        var update = Builders<User>.Update.Push("StudentPlan", subgoal);
        Console.WriteLine("Inserting subgoal into ALL students");
        await userCollection.UpdateManyAsync(filter, update);
    }

    // Indsætter subgoal i specifikke elever
    public async void InsertSubgoalSpecific(SubGoal subgoal, List<int> studentIds)
    {
        // For hvert studentId, indsæt delmålet
        foreach (var studentId in studentIds)
        {
            var filter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
            var update = Builders<User>.Update.Push("StudentPlan", subgoal);
            Console.WriteLine($"Inserting subgoal into student {studentId}");
            await userCollection.UpdateManyAsync(filter, update);
        }
    }
    
     public async Task<SubGoal> UpdateSubGoalDetails(SubGoal subGoal)
    {
    Console.WriteLine("Updating subgoal: repository");

    // Find studerende, der har dette subgoal i deres plan
    var filter = Builders<User>.Filter.And(
        Builders<User>.Filter.Eq("Role", "Student"),
        Builders<User>.Filter.ElemMatch(u => u.StudentPlan, s => s.SubGoalId == subGoal.SubGoalId)
    );

    var users = await userCollection.Find(filter).ToListAsync();
    
    foreach (var user in users)
    {
        // Gå igennem hele studentplan for hver bruger
        for (int i = 0; i < user.StudentPlan.Count; i++)
        {
            // Find det specifikke subgoal man prøver at updatere
            if (user.StudentPlan[i].SubGoalId == subGoal.SubGoalId)
            {
                // Opdater overordnet info
                user.StudentPlan[i].SubGoalName = subGoal.SubGoalName;
                user.StudentPlan[i].SubGoalDescription = subGoal.SubGoalDescription;
                user.StudentPlan[i].SubGoalType = subGoal.SubGoalType;

                // initiering af nye middle goals
                var updatedMiddleGoals = new List<MiddleGoal>();

                // gå igennem alle middlegoals i updaterede subgoal
                foreach (var newMiddle in subGoal.MiddleGoals)
                {
                    // check om middlegoal med samme navn allerede eksisterer i tidligere subgoal
                    var existingMiddle = user.StudentPlan[i].MiddleGoals
                        ?.FirstOrDefault(m => m.Name == newMiddle.Name);

                    // initiering af nye minigoals
                    var updatedMiniGoals = new List<MiniGoal>();

                    // gå igennem alle minigoals i nye subgoal
                    foreach (var newMini in newMiddle.MiniGoals)
                    {
                        // check om minigoal med samme navn allerede eksisterer i tidligere subgoals middlegoal
                        var existingMini = existingMiddle?.MiniGoals
                            ?.FirstOrDefault(m => m.Name == newMini.Name);
                        
                        // tilføj nye minigoals til initierede liste (linje 325)
                        updatedMiniGoals.Add(new MiniGoal
                        {
                            Name = newMini.Name,
                            // hvis minigoal allerede eksisterede og status er true, behold true status. Ellers er det et nyt navn til minigoal og status bliver false
                            Status = newMini.Status || existingMini?.Status == true
                        });
                    }

                    // tilføj nye middlegoals til initierede liste (linje 315)
                    updatedMiddleGoals.Add(new MiddleGoal
                    {
                        Name = newMiddle.Name,
                        // minigoals bliver til listen af updaterede minigoals
                        MiniGoals = updatedMiniGoals
                    });
                }

                // middlegoals bliver den updaterede liste af middlegoals
                user.StudentPlan[i].MiddleGoals = updatedMiddleGoals;
            }
        }

        await userCollection.ReplaceOneAsync(u => u.UserId == user.UserId, user);
    }

    return subGoal;
}
    
    

    public async void CompleteSubGoalBySubGoalId(int subGoalId, int studentId)
    {
        // updater i User
        var userFilter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
        var subgoalsFilter = Builders<User>.Filter.ElemMatch(x => x.StudentPlan, g => g.SubGoalId == subGoalId);
        var combinedFilter = Builders<User>.Filter.And(userFilter, subgoalsFilter);
        var userUpdate = Builders<User>.Update.Set("StudentPlan.$.Status", true);
        await userCollection.UpdateOneAsync(combinedFilter, userUpdate);

    }

    public async void DeleteSubGoalBySubGoalId(int subGoalId)
    {
        // sletter i subgoal collection
        var filter = Builders<SubGoal>.Filter.Eq(x => x.SubGoalId, subGoalId);
        await subCollection.DeleteOneAsync(filter);
        
        // sletter i user collection
        var userFilter = Builders<User>.Filter.Eq("Role", "Student");
        var userUpdate = Builders<User>.Update.PullFilter(u => u.StudentPlan, g => g.SubGoalId == subGoalId);
        await userCollection.UpdateManyAsync(userFilter, userUpdate);
    }
    
}