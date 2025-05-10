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
        return subgoals;
        
    }

    public async Task<List<TemplateSubGoal>?> GetAllTemplateSubGoalsAsync()
    {
        var filter = Builders<TemplateSubGoal>.Filter.Empty;
        return await tempCollection.Find(filter).ToListAsync();
    }

    public void CreateSubgoal()
    {

    }

    public void AddSubGoalToTemplates()
    {

    }

    public void UpdateSubGoalDetails(SubGoal subGoal)
    {

    }

    public void UpdateSubGoalDetailsTemplates(TemplateSubGoal template)
    {
        
    }

    public void CompleteSubGoalBySubGoalId(int subGoalId)
    {
        
    }

    public void DeleteSubGoalBySubGoalId(int subGoalId)
    {
        
    }

    public void DeleteTemplateByTemplateId(int templateId)
    {
        
    }
}