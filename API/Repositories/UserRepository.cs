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

    public async Task AddUserAsync(User user)
    {
        user.UserId = await GetMaxUserId() + 1;
        if (user.Role == "Student")
        {
            await _collection.InsertOneAsync(user);
            
            var filter = Builders<SubGoal>.Filter.Eq(x => x.SubGoalType, "Standard");
            var standardSubGoals = await _subGoalCollection.Find(filter).ToListAsync();

            var userFilter = Builders<User>.Filter.Eq(x => x.UserId, user.UserId);
            
            foreach (var subGoal in standardSubGoals)
            {
                var userYear = user.StartDate.HasValue ? user.StartDate.Value.Year : 0;
                subGoal.SubGoalDueDate = subGoal.SubGoalDueDate.AddYears(userYear - subGoal.SubGoalDueDate.Year);
                var userUpdate = Builders<User>.Update.Push("StudentPlan", subGoal);
                await _collection.UpdateOneAsync(userFilter, userUpdate);
            }
        }
        else
        {
            await _collection.InsertOneAsync(user);
        }
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


}
