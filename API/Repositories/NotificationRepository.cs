using API.Repositories.Interface;
using Core;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {   
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationlRepository;
        private readonly ISubGoalRepository _subgoalRepository;
        private readonly string _connectionString;
        
        private readonly MongoClient _client;
        private readonly IMongoCollection<User> _userCollection;
        
        public NotificationRepository(ISubGoalRepository subgoalRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _subgoalRepository = subgoalRepository;

            // Connection til database. Kunne også skrive den i launchSettings.json?
            _connectionString = "mongodb+srv://mortenstnielsen:hlEgCKJrN89edDQt@clusterfree.a2y2b.mongodb.net/";
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("No connection string configured");
            }

            _client = new MongoClient(_connectionString);
            var db = _client.GetDatabase("Comwell");
            _userCollection = db.GetCollection<User>("Users");
        }

        // Create
        public async Task SendMiniGoalNotificationAsync(Notification notification)
        {
            notification.NotificationId = await GetMaxNotificationIdAsync() + 1;
            notification.TimeStamp = DateTime.UtcNow;

            var filter = Builders<User>.Filter.Eq(u => u.UserId, notification.UserId);
            var update = Builders<User>.Update.Push(u => u.Notifications, notification);

            var result = await _userCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new Exception($"Ingen bruger med UserId {notification.UserId} fundet");
            }

            if (result.ModifiedCount == 0)
            {
                throw new Exception($"Notification blev ikke tilføjet til bruger {notification.UserId}");
            }
            
        }
        
        public async Task SendMiddleGoalNotificationAsync(Notification notification)
        {
            notification.NotificationId = await GetMaxNotificationIdAsync() + 1;
            notification.TimeStamp = DateTime.UtcNow;

            var filter = Builders<User>.Filter.Eq(u => u.UserId, notification.UserId);
            var update = Builders<User>.Update.Push(u => u.Notifications, notification);

            var result = await _userCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new Exception($"Ingen bruger med UserId {notification.UserId} fundet");
            }

            if (result.ModifiedCount == 0)
            {
                throw new Exception($"Notification blev ikke tilføjet til bruger {notification.UserId}");
            }
            
        }
        // Subgoals
      public async Task<UpdateResult> UpdateMiniGoalAndRemoveNotificationAsync(int userId, string miniGoalName, int notificationId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);

            var update = Builders<User>.Update
                .Set("StudentPlan.$[plan].MiddleGoals.$[middle].MiniGoals.$[mini].Status", true)
                .PullFilter(u => u.Notifications, n => n.NotificationId == notificationId);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("mini.Name", miniGoalName)
                ),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("middle.MiniGoals",
                        new BsonDocument("$elemMatch", new BsonDocument("Name", miniGoalName)))
                ),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("plan.MiddleGoals",
                        new BsonDocument("$elemMatch",
                            new BsonDocument("MiniGoals",
                                new BsonDocument("$elemMatch", new BsonDocument("Name", miniGoalName)))))
                )
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            // Debug: Udskriv miniGoalName
            Console.WriteLine($"miniGoalName fra notifikation: '{miniGoalName}'");

            // Hent bruger for at verificere mini goal findes
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();

            if (user != null && user.StudentPlan != null)
            {
                bool miniGoalExists = false;
                foreach (var plan in user.StudentPlan)
                {
                    Console.WriteLine($"➡️ SubGoal: {plan.SubGoalName}");
                    if (plan.MiddleGoals != null)
                    {
                        foreach (var middle in plan.MiddleGoals)
                        {
                            Console.WriteLine($"  ↪️ MiddleGoal: {middle.Name}");
                            if (middle.MiniGoals != null)
                            {
                                foreach (var mini in middle.MiniGoals)
                                {
                                    Console.WriteLine($"    • MiniGoal: '{mini.Name}', Status: {mini.Status}");
                                    if (string.Equals(mini.Name?.Trim(), miniGoalName?.Trim(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        miniGoalExists = true;
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine(miniGoalExists
                    ? $"✅ MiniGoal '{miniGoalName}' findes og forsøges opdateret."
                    : $"❌ MiniGoal '{miniGoalName}' blev ikke fundet i brugerens struktur.");
            }
            else
            {
                Console.WriteLine("❌ Bruger ikke fundet eller StudentPlan er tom.");
            }

            var result = await _userCollection.UpdateOneAsync(filter, update, options);

            Console.WriteLine($"📊 MongoDB ModifiedCount: {result.ModifiedCount}");

            return result;
        }

        
        public async Task<UpdateResult> UpdateMiddleGoalAndRemoveNotificationAsync(int userId, string middleGoalName, int notificationId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);

            var update = Builders<User>.Update
                .Set("StudentPlan.$[sub].MiddleGoals.$[middle].Status", true)
                .PullFilter(u => u.Notifications, n => n.NotificationId == notificationId);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new JsonArrayFilterDefinition<BsonDocument>("{ 'sub.MiddleGoals': { $exists: true } }"),
                new JsonArrayFilterDefinition<BsonDocument>("{ 'middle.Name': '" + middleGoalName + "' }")
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };
            
            Console.WriteLine("MongoDB update command:");
            Console.WriteLine($"UserId: {userId}");
            Console.WriteLine($"MiddleGoalName: {middleGoalName}");
            Console.WriteLine($"NotificationId: {notificationId}");

            
            
            return await _userCollection.UpdateOneAsync(filter, update, options);
        }



        // User
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            var user = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            return user?.Notifications.OrderByDescending(n => n.TimeStamp).ToList() ?? new List<Notification>();
        }
        


        
        // Id
        public async Task<int> GetMaxNotificationIdAsync()
        {
            var users = await _userCollection.Find(_ => true).ToListAsync();
            return users.SelectMany(u => u.Notifications).Max(n => (int?)n.NotificationId) ?? 0;
        }
        public async Task<bool> NotificationExistsForMiniGoalAsync(int userId, int senderId, string miniGoalName)
        {
            var user = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (user == null || user.Notifications == null)
                return false;

            return user.Notifications.Any(n =>
                n.SenderId == senderId &&
                !n.IsConfirmed &&
                n.MiniGoalName == miniGoalName);
        }
        
        public async Task<bool> NotificationExistsForMiddleGoalAsync(int userId, int senderId, string middleGoalName)
        {
            var user = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (user == null || user.Notifications == null)
                return false;

            return user.Notifications.Any(n =>
                n.SenderId == senderId &&
                !n.IsConfirmed &&
                n.MiddleGoalName == middleGoalName);
        }
        

        
    }
}