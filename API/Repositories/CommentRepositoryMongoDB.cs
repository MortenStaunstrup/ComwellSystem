using API.Repositories.Interface;
using Core;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace API.Repositories;

public class CommentRepositoryMongoDB : ICommentRepository
{
    private readonly string _connectionString;
    private IMongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<SubGoal> subCollection;
    private IMongoCollection<User> userCollection;
    private IMongoCollection<Comment> commentCollection;

    public CommentRepositoryMongoDB()
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
        commentCollection = database.GetCollection<Comment>("Comments");
    }
    
    public async Task<List<Comment>?> GetCommentsBySubGoalId(int subGoalId)
    {
        var filter = Builders<Comment>.Filter.Eq(x => x.CommentSubGoalId, subGoalId);
        var result = commentCollection.Find(filter).ToList();
        result.Sort((x, y) => x.CommentDate.CompareTo(y.CommentDate));
        
        if (result != null)
        {
            Console.WriteLine("Returning comments: repository");
            return result;
        }
        Console.WriteLine("No comments found, returning null: respository");
        return result;
    }

    public async void AddComment(Comment comment)
    {
        comment.CommentId = await MaxCommentId() + 1;
        Console.WriteLine("Adding comment: repository");
        await commentCollection.InsertOneAsync(comment);
        
        // Tilføjer comment i studentplan subgoal

        var filter = Builders<User>.Filter.Eq(x => x.UserId, comment.CommentSenderId);
        var subGoalFilter = Builders<User>.Filter.ElemMatch(u => u.StudentPlan, s => s.SubGoalId == comment.CommentSubGoalId);
        var combinedFilter = Builders<User>.Filter.And(filter, subGoalFilter);

        var update = Builders<User>.Update.Push("StudentPlan.$.Comments", comment);
        
        await userCollection.UpdateOneAsync(combinedFilter, update);

    }

    public async Task<int> MaxCommentId()
    {
        var sort = Builders<Comment>.Sort.Descending(x => x.CommentId);
        var maxSubGoalId = await commentCollection
            .Find(Builders<Comment>.Filter.Empty)
            .Sort(sort)
            .Limit(1)
            .FirstOrDefaultAsync();
        return maxSubGoalId?.CommentId ?? 0;
    }
}