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
    private GridFSBucket bucket;

    public SubGoalRepositoryMongoDB()
    {
        _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("No connection string configured");
        }
        client = new MongoClient(_connectionString);
        database = client.GetDatabase("Comwell");
        subCollection = database.GetCollection<SubGoal>("SubGoals");
        userCollection = database.GetCollection<User>("Users");
        bucket = new GridFSBucket(database, new GridFSBucketOptions{ BucketName = "Comwell files" });
    }
    
    public async Task<List<SubGoal>?> GetNotCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var studentFilter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
        var subGoalFilter = Builders<User>.Filter.ElemMatch(x => x.StudentPlan, s => s.SubGoalStatus == false);
        var combinedFilter = Builders<User>.Filter.And(studentFilter, subGoalFilter);
        var projection = Builders<User>.Projection.Exclude("_id").Include("StudentPlan");
        
        var result = await userCollection
            .Find(combinedFilter)
            .Project(projection)
            .FirstOrDefaultAsync();

        var subgoals = result["StudentPlan"]
            .AsBsonArray
            .Select(subgoal => BsonSerializer.Deserialize<SubGoal>(subgoal.ToBsonDocument()))
            .ToList();
        
        Console.WriteLine($"Returing unfinished subgoals for student {studentId}");
        return subgoals;
    }

    public async Task<List<SubGoal>?> GetCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var studentFilter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
        var subGoalFilter = Builders<User>.Filter.ElemMatch(x => x.StudentPlan, s => s.SubGoalStatus == true);
        var combinedFilter = Builders<User>.Filter.And(studentFilter, subGoalFilter);
        var projection = Builders<User>.Projection.Exclude("_id").Include("StudentPlan");
        
        var result = await userCollection
            .Find(combinedFilter)
            .Project(projection)
            .FirstOrDefaultAsync();

        var subgoals = result["StudentPlan"]
            .AsBsonArray
            .Select(subgoal => BsonSerializer.Deserialize<SubGoal>(subgoal.ToBsonDocument()))
            .ToList();
        
        Console.WriteLine($"Returing finished subgoals for student {studentId}");
        return subgoals;
    }

    public async Task<double> GetPctCompletedSubGoalsByStudentIdAsync(int studentId)
    {
        var studentFilter = Builders<User>.Filter.Eq("_id", studentId);

        var aggregation = await userCollection.Aggregate()
            .Match(studentFilter)
            .Unwind(u => u.StudentPlan)
            .Group(new BsonDocument
            {
                { "_id", "$StudentPlan.SubGoalStatus" }, // Correct access to the subgoal status
                { "Count", new BsonDocument("$sum", 1) }
            })
            .ToListAsync();

        // Initialize counters
        int totalCount = 0;
        int completedCount = 0;

        // Calculate completed and total count
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
    

    public void UpdateSubGoalDetails(SubGoal subGoal)
    {
        
    }

    public async void CompleteSubGoalBySubGoalId(int subGoalId, int studentId)
    {
        // updater i SubGoal collection
        var filter = Builders<SubGoal>.Filter.Eq(x => x.SubGoalId, subGoalId);
        var subcollUserFilter = Builders<SubGoal>.Filter.Eq(x => x.StudentId, studentId);
        var userCollCombinedFilter = Builders<SubGoal>.Filter.And(filter, subcollUserFilter);
        var update = Builders<SubGoal>.Update.Set(x => x.SubGoalStatus, true);
        await subCollection.UpdateOneAsync(userCollCombinedFilter, update);
        
        // updater i User
        var userFilter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
        var subgoalsFilter = Builders<User>.Filter.ElemMatch(x => x.StudentPlan, g => g.SubGoalId == subGoalId);
        var combinedFilter = Builders<User>.Filter.And(userFilter, subgoalsFilter);
        var userUpdate = Builders<User>.Update.Set("StudentPlan.$.Status", true);
        await userCollection.UpdateOneAsync(combinedFilter, userUpdate);

    }

    public void DeleteSubGoalBySubGoalId(int subGoalId, int studentId)
    {
        
    }

    //TODO Skal ind i userRepo, controller og service
    public async void CreateUser(User user)
    {
        //Funktion til max User id indsættes her
        user.UserId = 1;
        if (user.Role == "Student")
        {
            await userCollection.InsertOneAsync(user);
            
            var filter = Builders<SubGoal>.Filter.Eq(x => x.SubGoalType, "Standard");
            var standardSubGoals = await subCollection.Find(filter).ToListAsync();

            var userFilter = Builders<User>.Filter.Eq(x => x.UserId, user.UserId);
            
            foreach (var subGoal in standardSubGoals)
            {
                subGoal.StudentId = user.UserId;
                var userYear = user.StartDate.HasValue ? user.StartDate.Value.Year : 0;
                subGoal.SubGoalDueDate = subGoal.SubGoalDueDate.AddYears(userYear - subGoal.SubGoalDueDate.Year);
                var userUpdate = Builders<User>.Update.Push("StudentPlan", subGoal);
                await userCollection.UpdateOneAsync(userFilter, userUpdate);
            }
            
        }
    }
    
}