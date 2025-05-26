using Core;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

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
    private GridFSBucket _bucket;

    public UserRepository()
    {
        _client = new MongoClient(connectionString);
        database = _client.GetDatabase("Comwell");
        _collection = database.GetCollection<User>("Users");
        _subGoalCollection = database.GetCollection<SubGoal>("SubGoals");
        _bucket = new GridFSBucket(database, new GridFSBucketOptions{BucketName = "Profile Pictures"});
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = await _collection.Find(new BsonDocument()).ToListAsync();
        foreach (var user in users)
        {
            if (user.PictureId != null)
            {
                user.Picture = Convert.ToBase64String(await _bucket.DownloadAsBytesAsync(user.PictureId));
            }
        }
        return users;
    }

    public async Task<List<User>?> GetAllKitchenManagersAsync()
    {
        var filter = Builders<User>.Filter.Eq(x => x.Role, "KitchenManager");
        var projection = Builders<User>.Projection.Exclude("Notifications").Exclude("Messages").Exclude("UserPassword");
        var managers = await _collection.Find(filter).Project<User>(projection).ToListAsync();
        foreach (var manager in managers)
        {
            if (manager.PictureId != null)
            {
                manager.Picture = Convert.ToBase64String(await _bucket.DownloadAsBytesAsync(manager.PictureId));
            }
        }
        return managers;
    }
    
    public async Task<List<User>> GetAllStudentsAsync()
    {
        var students = await _collection.Find(new BsonDocument()).ToListAsync();
        foreach (var student in students)
        {
            if (student.PictureId != null)
            {
                student.Picture = Convert.ToBase64String(await _bucket.DownloadAsBytesAsync(student.PictureId));
            }
        }
        return students;
    }

    public async Task<User?> GetUserByUserId(int? userId)
    {
        Console.WriteLine($"Returning user: {userId}: repo");
        var filter = Builders<User>.Filter.Eq("_id", userId);
        var student = await _collection.Find(filter).FirstOrDefaultAsync();
        if (student != null && student.PictureId != null)
        {
            student.Picture = Convert.ToBase64String(await _bucket.DownloadAsBytesAsync(student.PictureId));
        }
        return student;
    }

    public async Task AddUserAsync(User user)
    {
        user.UserId = await GetMaxUserId() + 1;
        if (user.Picture != null)
        {
            var picId = await _bucket.UploadFromBytesAsync(user.UserName,Convert.FromBase64String(user.Picture));
            user.PictureId = picId;
            user.Picture = null;
        }
        await _collection.InsertOneAsync(user);
        if (user.Role == "Student")
            InsertAllStandardSubGoalsInStudent(user);
    }

    public async Task<List<User>?> GetAllStudentsByResponsibleIdAsync(int responsibleId)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserIdResponsible, responsibleId);
        var projection = Builders<User>.Projection.Exclude("Notifications").Exclude("Messages").Exclude("UserPassword");
        var students = await _collection.Find(filter).Project<User>(projection).ToListAsync();
        foreach (var student in students)
        {
            if (student.PictureId != null)
            {
                student.Picture = Convert.ToBase64String(await _bucket.DownloadAsBytesAsync(student.PictureId));
            }
        }
        return students;
    }

    public async void InsertAllStandardSubGoalsInStudent(User user)
    {
        try
        {
            //henter alle subgoals i collection der er standard, og sætter dem ind i elevens plan
            var filter = Builders<SubGoal>.Filter.Eq(sg => sg.SubGoalType, "Standard");
            var standardSubGoals = await _subGoalCollection.Find(filter).ToListAsync();

            if (standardSubGoals == null || !standardSubGoals.Any())
            {
                Console.WriteLine("Ingen standard SubGoals fundet.");
                return;
            }
            
            user.StudentPlan.AddRange(standardSubGoals); //tilføjer til studentplan listen på én gang
            
            var userFilter = Builders<User>.Filter.Eq(u => u.UserId, user.UserId);
            var update = Builders<User>.Update.Set(u => u.StudentPlan, user.StudentPlan);

            await _collection.UpdateOneAsync(userFilter, update);

            Console.WriteLine($"Standard SubGoals tilføjet til bruger {user.UserId}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fejl i InsertAllStandardSubGoalsInStudent: {ex.Message}");
        }
    }

    
    public async Task<User?> Login(string email, string password)
    {
        try
        {
            Console.WriteLine($"Login attempt with email: {email}");

            var emailFilter = Builders<User>.Filter.Regex("UserEmail", new BsonRegularExpression(email, "i"));
            var usersWithEmail = await _collection.Find(emailFilter).ToListAsync();
            Console.WriteLine($"Found {usersWithEmail.Count} users with matching email.");

            foreach (var u in usersWithEmail)
            {
                Console.WriteLine($"User: {u.UserEmail}, Password: {u.UserPassword}, Role: {u.Role}");
            }

            var passwordFilter = Builders<User>.Filter.Eq("UserPassword", password);
            var combinedFilter = Builders<User>.Filter.And(emailFilter, passwordFilter);

            var user = await _collection.Find(combinedFilter).FirstOrDefaultAsync();

            if (user == null)
            {
                Console.WriteLine("Login failed: No user matched both email and password.");
            }
            else
            {
                Console.WriteLine($"Login success: UserId={user.UserId}, Role={user.Role}");
            }
            if(user.PictureId != null)
                user.Picture = Convert.ToBase64String(await _bucket.DownloadAsBytesAsync(user.PictureId));
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during login: {ex.Message}");
            return null;
        }
    }




    public async Task<User> GetUserByLoginAsync(int userId)
    {
        var user = await _collection.Find(new BsonDocument("_id", userId)).FirstOrDefaultAsync();
        if(user != null && user.PictureId != null)
        {
            user.Picture = Convert.ToBase64String(await _bucket.DownloadAsBytesAsync(user.PictureId));
        }
        return user;
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
        if (user.Picture == null)
        {
            dbUser.Picture = null;
            if(dbUser.PictureId != null)
                await _bucket.DeleteAsync(dbUser.PictureId);
            dbUser.PictureId = null;
        }
        else
        {
            dbUser.PictureId = await _bucket.UploadFromBytesAsync(user.UserName,Convert.FromBase64String(user.Picture));
        }
        var filter = Builders<User>.Filter.Eq(u => u.UserId, user.UserId);
        await _collection.ReplaceOneAsync(filter, dbUser);
    }
}
