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
    
    /// <summary>
    /// Konstruktør. Opretter forbindelse til MongoDB og initialiserer collections og GridFS.
    /// </summary>
    public UserRepository()
    {
        _client = new MongoClient(connectionString);
        database = _client.GetDatabase("Comwell");
        _collection = database.GetCollection<User>("Users");
        _subGoalCollection = database.GetCollection<SubGoal>("SubGoals");
        _bucket = new GridFSBucket(database, new GridFSBucketOptions{BucketName = "Profile Pictures"});
    }

    /// <summary>
    /// Henter alle brugere fra databasen, inklusive deres profilbilleder.
    /// </summary>
    /// <returns>En liste af alle brugere.</returns>
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
    
    /// <summary>
    /// Henter alle brugere med rollen "KitchenManager" (køkkenleder), uden deres notifikationer, beskeder og adgangskode.
    /// </summary>
    /// <returns>En liste af køkkenledere eller null, hvis ingen findes.</returns>
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
    
    /// <summary>
    /// Henter alle brugere, der er studerende.
    /// </summary>
    /// <returns>En liste af studerende.</returns>
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

    /// <summary>
    /// Henter en bruger baseret på deres bruger-ID.
    /// </summary>
    /// <param name="userId">Brugerens ID.</param>
    /// <returns>Et User-objekt eller null, hvis ingen bruger findes.</returns>
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

    /// <summary>
    /// Tilføjer en ny bruger til databasen og uploader eventuelt profilbillede.
    /// Hvis brugeren er studerende, tilføjes standard-subgoals.
    /// </summary>
    /// <param name="user">Brugeren der skal tilføjes.</param>
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

    
    /// <summary>
    /// Henter alle studerende, som har en specifik ansvarlig (userId).
    /// </summary>
    /// <param name="responsibleId">userId for ansvarlig person.</param>
    /// <returns>En liste af studerende med given ansvarlig.</returns>
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

    
    /// <summary>
    /// Tilføjer alle standard-subgoals til en given studerendes læringsplan.
    /// </summary>
    /// <param name="user">Studerende som skal have subgoals tilføjet.</param>
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
            
            //tilføjer til studentplan listen på én gang
            user.StudentPlan.AddRange(standardSubGoals);
            
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

    
    /// <summary>
    /// Logger en bruger ind ved at matche email og adgangskode.
    /// </summary>
    /// <param name="email">Brugerens email.</param>
    /// <param name="password">Brugerens adgangskode.</param>
    /// <returns>Brugeren hvis login lykkedes; ellers null.</returns>
    public async Task<User?> Login(string email, string password)
    {
        try
        {
            var emailFilter = Builders<User>.Filter.Regex("UserEmail", new BsonRegularExpression(email, "i"));
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



    /// <summary>
    /// Henter en bruger ved login baseret på deres brugerID.
    /// </summary>
    /// <param name="userId">Brugerens ID.</param>
    /// <returns>Bruger med angivet ID.</returns>
    public async Task<User> GetUserByLoginAsync(int userId)
    {
        var user = await _collection.Find(new BsonDocument("_id", userId)).FirstOrDefaultAsync();
        if(user != null && user.PictureId != null)
        {
            user.Picture = Convert.ToBase64String(await _bucket.DownloadAsBytesAsync(user.PictureId));
        }
        return user;
    }
    
    /// <summary>
    /// Finder det højeste bruger-ID i databasen.
    /// </summary>
    /// <returns>Det højeste UserId (som er et heltal), eller 0 hvis ingen findes.</returns>
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
    
    /// <summary>
    /// Opdaterer en bruger i databasen, inklusive profilbillede.
    /// </summary>
    /// <param name="user">Brugeren der skal opdateres.</param>
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
            dbUser.Picture = null;
        }
        var filter = Builders<User>.Filter.Eq(u => u.UserId, user.UserId);
        await _collection.ReplaceOneAsync(filter, dbUser);
    }
    
    
    /// <summary>
    /// Sletter en bruger fra databasen samt deres profilbillede, hvis det findes.
    /// </summary>
    /// <param name="userId">Brugerens ID.</param>
    public async Task DeleteUserAsync(int userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);
    
        var user = await _collection.Find(filter).FirstOrDefaultAsync();
        if (user?.PictureId != null)
        {
            await _bucket.DeleteAsync(user.PictureId);
        }

        await _collection.DeleteOneAsync(filter);
    }

}