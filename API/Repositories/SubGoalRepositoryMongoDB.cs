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
            .Group(new BsonDocument
            {
                { "_id", "$StudentPlan.SubGoalStatus" },
                { "Count", new BsonDocument("$sum", 1) } // Gruper SubGoalStatus og et 1 tal.
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

        var percentage = (completedCount / (double)totalCount) * 100;
        Console.WriteLine($"The result for student {studentId} is {percentage}");
        return percentage;
    }

    public async Task<List<SubGoal>?> GetOfferedSubGoalsByStudentIdAsync(int studentId)
    {
        var studentFilter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
        var subGoalFilter = Builders<User>.Filter.ElemMatch(x => x.StudentPlan, s => s.SubGoalType == "Extra");
        var combinedFilter = Builders<User>.Filter.And(studentFilter, subGoalFilter);
        var projection = Builders<User>.Projection.Include("StudentPlan").Exclude("_id");
        
        var user = await userCollection
            .Find(combinedFilter)
            .Project<User>(projection)
            .FirstOrDefaultAsync();

        if (user?.StudentPlan == null)
            return null;
        
        Console.WriteLine($"Returning extra subgoals for student {studentId}");
        return user.StudentPlan.ToList();
    }

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
        var maxSubGoalId = await subCollection
            .Find(Builders<SubGoal>.Filter.Empty)
            .Sort(sort)
            .Limit(1)
            .FirstOrDefaultAsync();
        return maxSubGoalId?.SubGoalId ?? 0;
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
        foreach (var studentId in studentIds)
        {
            var filter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
            var update = Builders<User>.Update.Push("StudentPlan", subgoal);
            Console.WriteLine($"Inserting subgoal into student {studentId}");
            await userCollection.UpdateManyAsync(filter, update);
        }
    }


    public async void UpdateSubGoalDetails(SubGoal subGoal)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq("Role", "Student"),
            Builders<User>.Filter.ElemMatch(u => u.StudentPlan, s => s.SubGoalId == subGoal.SubGoalId)
        );

        var update = Builders<User>.Update
            .Set("StudentPlan.$[elem].SubGoalName", subGoal.SubGoalName)
            .Set("StudentPlan.$[elem].SubGoalDescription", subGoal.SubGoalDescription)
            .Set("StudentPlan.$[elem].SubGoalType", subGoal.SubGoalType)
            .Set("StudentPlan.$[elem].MiddleGoals", subGoal.MiddleGoals);

        var arrayFilters = new List<ArrayFilterDefinition>
        {
            new JsonArrayFilterDefinition<User>("{ 'elem.SubGoalId': " + subGoal.SubGoalId + " }")
        };

        var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

        await userCollection.UpdateManyAsync(filter, update, updateOptions);

    }
    
    
   
    /* public async void UpdateSubGoalDetails(SubGoal subGoal)
    {
        // updater i subgoals collection
        var filterSubGoal = Builders<SubGoal>.Filter.Eq(x => x.SubGoalId, subGoal.SubGoalId);
        subCollection.ReplaceOne(filterSubGoal, subGoal);
        
        // updater i users
        var filterUserSubGoal = Builders<User>.Filter.ElemMatch(x => x.StudentPlan, s => s.SubGoalId == subGoal.SubGoalId);
        var studentFilter = Builders<User>.Filter.Eq("Role", "Student");
        var combined = Builders<User>.Filter.And(filterUserSubGoal, studentFilter);
        var pushUpdate = Builders<User>.Update.Push("StudentPlan", subGoal);
        var pullUpdate = Builders<User>.Update.PullFilter(u => u.StudentPlan, s => s.SubGoalId == subGoal.SubGoalId);
        
        // Først pull den gamle version ud, derefter push den nye
        await userCollection.UpdateManyAsync(combined, pullUpdate);
        await userCollection.UpdateManyAsync(studentFilter, pushUpdate);
        
        // todo problem: hvis man prøver at updater i users(students), hvordan beholder man deres status på delmålet og kun
        // ændrer navnet og strukturen?? idk
        // måske gør i c#??? ville måske virke hvis det var i én elev man updateret delmålet så nej
    }*/

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