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
        _connectionString = "mongodb+srv://mortenstnielsen:hlEgCKJrN89edDQt@clusterfree.a2y2b.mongodb.net/";
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
    
    public async Task<List<Comment>?> GetCommentsBySubGoalId(int subGoalId, int studentId)
    {
        // finder kommentar tilhørende specifik elev og subgoal combo
        // hvorfor tager vi den ikke fra studentplan i elev?? idk tbh
        var filter = Builders<Comment>.Filter.Eq(x => x.CommentSubGoalId, subGoalId);
        var userFilter = Builders<Comment>.Filter.Eq(x => x.StudentId, studentId);
        var combinedFilter = Builders<Comment>.Filter.And(filter, userFilter);
        var result = commentCollection.Find(combinedFilter).ToList();
        // sorterer resultatet efter CommentDate, så nyeste beskeder kommer sidst
        result.Sort((x, y) => x.CommentDate.CompareTo(y.CommentDate));
        
        // check til debugging
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
        // tilføjer til comment collection
        comment.CommentId = await MaxCommentId() + 1;
        Console.WriteLine("Adding comment: repository");
        await commentCollection.InsertOneAsync(comment);
        
        // Tilføjer comment i studentplan subgoal

        var filter = Builders<User>.Filter.Eq(x => x.UserId, comment.StudentId);
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