using Core;
using MongoDB.Driver;
using MongoDB.Bson;
namespace API.Repositories;
 // Har prøvet at organisere lidt i koden, ved ikke om det giver mening at opdele det med kommentarer.
public class UserRepository : IUserRepository
{
    // 1. DATABASESETUP
    private readonly string _connectionString;
    private readonly MongoClient _client;
    private readonly IMongoCollection<User> userCollection;
    private readonly IMongoCollection<SubGoal> subCollection;
    private readonly IMongoCollection<Notification> notificationCollection;

    public UserRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("No connection string configured");
        }
        _client = new MongoClient("mongodb+srv://Hjalte:Hjalte123@clusterfree.a2y2b.mongodb.net/");
        var db = _client.GetDatabase("Comwell");

        userCollection = db.GetCollection<User>("Users");
        subCollection = db.GetCollection<SubGoal>("SubGoals");
        notificationCollection = db.GetCollection<Notification>("Notifications");
    }

    // 2. BRUGER

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await userCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<List<User>> GetAllStudentsAsync()
    {
        var filter = Builders<User>.Filter.Eq("Role", "Student");
        return await userCollection.Find(filter).ToListAsync();
    }

    public async Task<User?> Login(string email, string password)
    {
        var filterEmail = Builders<User>.Filter.Eq("UserEmail", email);
        var filterPassword = Builders<User>.Filter.Eq("UserPassword", password);
        var filter = Builders<User>.Filter.And(filterEmail, filterPassword);

        return await userCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserByLoginAsync(int userId)
    {
        var filter = Builders<User>.Filter.Eq("_id", userId);
        return await userCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<int> GetMaxUserId()
    {
        var sort = Builders<User>.Sort.Descending(x => x.UserId);
        var user = await userCollection.Find(Builders<User>.Filter.Empty).Sort(sort).Limit(1).FirstOrDefaultAsync();
        return user?.UserId ?? 0;
    }

    public async Task AddUserAsync(User user)
    {
        user.UserId = await GetMaxUserId() + 1;
        await userCollection.InsertOneAsync(user);

        if (user.Role == "Student")
        {
            InsertAllStandardSubGoalsInStudent(user);
        }
    }

    // 3. SUBGOALS OG NOTIFIKATIONER

    public async Task EmbedNotificationToUserAsync(User user, Notification notification)
    {
        var filter = Builders<User>.Filter.Eq(u => u.UserId, user.UserId);
        var update = Builders<User>.Update.Push("Notifications", notification);
        await userCollection.UpdateOneAsync(filter, update);
    }

    public async Task<int> GetEarliestYearForStandardSubGoals()
    {
        var filter = Builders<SubGoal>.Filter.Eq(x => x.SubGoalType, "Standard");
        var sort = Builders<SubGoal>.Sort.Ascending(x => x.SubGoalDueDate);
        var result = await subCollection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
        return result.SubGoalDueDate.Year;
    }

    public async Task<List<User>?> GetAllStudentsByResponsibleIdAsync(int responsibleId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserIdResponsible, responsibleId);
        var projection = Builders<User>.Projection.Exclude("Notifications").Exclude("Messages").Exclude("UserPassword");
        return await userCollection.Find(filter).Project<User>(projection).ToListAsync();
    }

    public async void InsertAllStandardSubGoalsInStudent(User user)
    {
        var filter = Builders<SubGoal>.Filter.Eq(x => x.SubGoalType, "Standard");
        var standardSubGoals = await subCollection.Find(filter).ToListAsync();
        var userFilter = Builders<User>.Filter.Eq(x => x.UserId, user.UserId);

        var userYear = user.StartDate.HasValue ? user.StartDate.Value.Year : 0;
        var earliestSubGoalYear = await GetEarliestYearForStandardSubGoals();
        foreach (var subGoal in standardSubGoals)
        {
            // Calculate how many years have passed since the earliest standard subgoal year
            var yearsPassedSinceEarliestSubGoal = subGoal.SubGoalDueDate.Year - earliestSubGoalYear;

            // Set the new due date year based on the user's start year and how many years passed since the earliest
            var newDueDateYear = userYear + yearsPassedSinceEarliestSubGoal;

            // Update the subgoal's due date
            subGoal.SubGoalDueDate = new DateOnly(newDueDateYear, subGoal.SubGoalDueDate.Month, subGoal.SubGoalDueDate.Day);

            // Update user's student plan
            var userUpdate = Builders<User>.Update.Push("StudentPlan", subGoal);
            await userCollection.UpdateOneAsync(userFilter, userUpdate);
        }
    }
        }
    



