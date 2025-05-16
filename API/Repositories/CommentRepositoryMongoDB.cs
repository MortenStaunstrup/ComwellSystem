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
    
    public async Task<List<Comment>?> GetCommentsBySubGoalAndStudentId(int userId, int subGoalId)
    {
        var filter = Builders<Comment>.Filter.Eq(x => x.CommentSubGoalId, subGoalId);
        var result = commentCollection.Find(filter).ToList();
        if (result != null)
        {
            Console.WriteLine("Returning comments: repository");
            return result;
        }
        Console.WriteLine("No comments found, returning null: respository");
        return result;
    }

    public void AddComment(Comment comment)
    {
        commentCollection.InsertOne(comment);
    }
}