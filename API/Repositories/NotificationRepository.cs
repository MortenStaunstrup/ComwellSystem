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
        public async Task<bool> UpdateMiniGoalStatusAsync(int studentId, string miniGoalName)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, studentId);

            var update = Builders<User>.Update
                .Set("StudentPlan.$[plan].MiddleGoals.$[middle].MiniGoals.$[mini].Status", true);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("mini.Name", miniGoalName)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("middle.MiniGoals.Name", miniGoalName)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("plan.MiddleGoals.MiniGoals.Name", miniGoalName))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            var result = await _userCollection.UpdateOneAsync(filter, update, options);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveNotificationMiniGoalFromManagerAsync(int leaderId, int notificationId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, leaderId);

            var update = Builders<User>.Update
                .PullFilter(u => u.Notifications, n => n.NotificationId == notificationId);

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }


        public async Task<bool> UpdateMiddleGoalStatusAsync(int studentId, string middleGoalName)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, studentId);

            var update = Builders<User>.Update
                .Set("StudentPlan.$[].MiddleGoals.$[middle].Status", true);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new JsonArrayFilterDefinition<BsonDocument>("{ 'middle.Name': '" + middleGoalName + "' }")
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };
            var result = await _userCollection.UpdateOneAsync(filter, update, options);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveNotificationMiddleGoalFromManagerAsync(int leaderId, int notificationId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, leaderId);
            var update = Builders<User>.Update.PullFilter(u => u.Notifications, n => n.NotificationId == notificationId);

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
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