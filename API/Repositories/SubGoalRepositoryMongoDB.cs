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
    
    public async Task<List<SubGoal>?> GetSubGoalsByStudentIdAsync(int studentId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserId, studentId);
        var projection = Builders<User>.Projection.Exclude("_id").Include("StudentPlan");
        
        var result = await userCollection
            .Find(filter)
            .Project(projection)
            .FirstOrDefaultAsync();

        var subgoals = result["StudentPlan"]
            .AsBsonArray
            .Select(subgoal => BsonSerializer.Deserialize<SubGoal>(subgoal.ToBsonDocument()))
            .ToList();
        
        if (result.Any())
        {
            foreach (var subgoal in subgoals)
            {
                if(subgoal.PictureId != null)
                    subgoal.SubGoalPicture = Convert.ToBase64String(await bucket.DownloadAsBytesAsync(subgoal.PictureId));
            }
        }
        Console.WriteLine($"Returing subgoals for student {studentId}");
        return subgoals;
        
    }

    public async Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync()
    {
        var filter = Builders<TemplateSubGoal>.Filter.Empty;
        var result = await tempCollection.Find(filter).ToListAsync();

        if (result.Any())
        {
            foreach (var template in result)
            {
                if (template.PictureId != null)
                    template.TemplateSubGoalPicture = Convert.ToBase64String(await bucket.DownloadAsBytesAsync(template.PictureId));
            }
        }
        Console.WriteLine("Returning all templates");
        return result;
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
        subgoal.SubGoalId = await MaxSubGoalId() + 1;
        if (subgoal.SubGoalPicture != null)
        {
            var picId = await bucket.UploadFromBytesAsync(subgoal.SubGoalName,Convert.FromBase64String(subgoal.SubGoalPicture));
            subgoal.PictureId = picId;
            subgoal.SubGoalPicture = null;
        }
        await subCollection.InsertOneAsync(subgoal);
        Console.WriteLine($"Inserting subgoal {subgoal.SubGoalId} into subgoal collection");
        
        // Indsætter subgoal i User under studentplan
        var filter = Builders<User>.Filter.Eq(x => x.UserId, subgoal.StudentId);
        var update = Builders<User>.Update.Push("StudentPlan", subgoal);
        await userCollection.FindOneAndUpdateAsync(filter, update);
        Console.WriteLine($"Inserting into user {subgoal.StudentId}");
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
        if (template.TemplateSubGoalPicture != null)
        {
            var picId = await bucket.UploadFromBytesAsync(template.TemplateSubGoalName,Convert.FromBase64String(template.TemplateSubGoalPicture));
            template.PictureId = picId;
            template.TemplateSubGoalPicture = null;
        }
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
        var update = Builders<SubGoal>.Update.Set(x => x.SubGoalStatus, true);
        await subCollection.UpdateOneAsync(filter, update);
        
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