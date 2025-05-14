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
    private IMongoCollection<TemplateSubGoal> tempCollection;
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
        tempCollection = database.GetCollection<TemplateSubGoal>("Templates");
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

    public async Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync()
    {
        var filter = Builders<TemplateSubGoal>.Filter.Empty;
        var result = await tempCollection.Find(filter).ToListAsync();
        
        Console.WriteLine("Returning all templates");
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

    public async void CreateSubgoal(SubGoal subgoal, List<int> studentIds)
    {
        if (studentIds == null || studentIds.Count == 0 || subgoal.SubGoalType == "Standard")
        {
            // Indsætter subgoal i SubGoal collection, når den er standdart
            await subCollection.InsertOneAsync(subgoal);
            Console.WriteLine($"Inserting subgoal {subgoal.SubGoalId} into subgoal collection");
        }
        else
        {
            foreach (var id in studentIds)
            {
                // Indsætter subgoal for hver studerende
                subgoal.StudentId = id;
                await subCollection.InsertOneAsync(subgoal);
                Console.WriteLine($"Inserting subgoal {subgoal.SubGoalId} into subgoal collection for student {id}");
            }
        }
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
            subgoal.StudentId = studentId;
            var filter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
            var update = Builders<User>.Update.Push("StudentPlan", subgoal);
            Console.WriteLine($"Inserting subgoal into student {studentId}");
            await userCollection.UpdateManyAsync(filter, update);
        }
    }

    public async Task<int> MaxTemplateId()
    {
        var sort = Builders<TemplateSubGoal>.Sort.Descending(x => x.TemplateSubGoalId);
        var maxSubGoalId = await tempCollection
            .Find(Builders<TemplateSubGoal>.Filter.Empty)
            .Sort(sort)
            .Limit(1)
            .FirstOrDefaultAsync();
        return maxSubGoalId?.TemplateSubGoalId ?? 0;
    }
    
    public async void AddSubGoalToTemplates(TemplateSubGoal template)
    {
        template.TemplateSubGoalId = await MaxTemplateId() + 1;
        Console.WriteLine($"Inserting template {template.TemplateSubGoalId} into templates");
        await tempCollection.InsertOneAsync(template);
    }

    public void UpdateSubGoalDetails(SubGoal subGoal)
    {
        
    }

    public void UpdateSubGoalDetailsTemplates(TemplateSubGoal template)
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

    public void DeleteTemplateByTemplateId(int templateId)
    {
        
    }
}