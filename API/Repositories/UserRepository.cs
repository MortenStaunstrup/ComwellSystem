using Core;
using MongoDB.Driver;
using MongoDB.Bson;
namespace API.Repositories;
//repositories generelt skal kun kunne snakke med databasen. Man kan sige: Du snakker kun dansk, men vil virkeligt -
//tale med en englænder, som også kun snakker sit sprog. Repositories er tolken, som snakker begge sprog. Dog ved repositories -
//ingen ting om logik; kun get, add, delete osv.
public class UserRepository : IUserRepository
{                                       //bare min egen string, tror godt I kan erstatte med jeres.
    private string connectionString = "mongodb+srv://Hjalte:Hjalte123@clusterfree.a2y2b.mongodb.net/";
    private MongoClient _client;
    private IMongoDatabase database;
    private IMongoCollection<User> _collection;
    private IMongoCollection<SubGoal> _subGoalCollection;

    public UserRepository()
    {
        _client = new MongoClient(connectionString);
        database = _client.GetDatabase("Comwell");
        _collection = database.GetCollection<User>("Users");
        _subGoalCollection = database.GetCollection<SubGoal>("SubGoals");
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _collection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<List<User>?> GetAllKitchenManagersAsync()
    {
        var filter = Builders<User>.Filter.Eq(x => x.Role, "KitchenManager");
        var projection = Builders<User>.Projection.Exclude("Notifications").Exclude("Messages").Exclude("UserPassword");
        return await _collection.Find(filter).Project<User>(projection).ToListAsync();
    }
    
    public async Task<List<User>> GetAllStudentsAsync()
    {
        return await _collection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<User?> GetUserByUserId(int userId)
    {
        Console.WriteLine($"Returning user: {userId}: repo");
        var filter = Builders<User>.Filter.Eq("_id", userId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task AddUserAsync(User user)
    {
        user.UserId = await GetMaxUserId() + 1;
        await _collection.InsertOneAsync(user);
        if (user.Role == "Student")
            InsertAllStandardSubGoalsInStudent(user);
    }

    public async Task<List<User>?> GetAllStudentsByResponsibleIdAsync(int responsibleId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserIdResponsible, responsibleId);
        var projection = Builders<User>.Projection.Exclude("Notifications").Exclude("Messages").Exclude("UserPassword");
        return await _collection.Find(filter).Project<User>(projection).ToListAsync();
    }

    public async void InsertAllStandardSubGoalsInStudent(User user)
    {
    }
    
    public async Task<User?> Login(string email, string password)
    {
        var filterEmail = Builders<User>.Filter.Eq("UserEmail", email);
        var filterPassword = Builders<User>.Filter.Eq("UserPassword", password);
        var filterAnd = Builders<User>.Filter.And(filterEmail, filterPassword);
        return await _collection.Find(filterAnd).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserByLoginAsync(int userId)
    {
        return await _collection.Find(new BsonDocument("_id", userId)).FirstOrDefaultAsync();
    }
    
    public async Task<int> GetMaxUserId() 
    {
        try
        {
            var sort = Builders<User>.Sort.Descending(x => x.UserId);
            var maxUser = await _collection
                .Find(Builders<User>.Filter.Empty)
                .Sort(sort)
                .Limit(1)
                .FirstOrDefaultAsync();
            return maxUser?.UserId ?? 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fejl i GetMaxUserId(): {ex.Message}");
            throw;
        }
    }
    public async Task UpdateUserAsync(User user)
    {
        var dbUser = await GetUserByUserId(user.UserId);
        dbUser.UserPhone = user.UserPhone;
        dbUser.UserEmail = user.UserEmail;
        dbUser.UserName = user.UserName;
        var filter = Builders<User>.Filter.Eq(u => u.UserId, user.UserId);
        await _collection.ReplaceOneAsync(filter, dbUser);
    }


}
