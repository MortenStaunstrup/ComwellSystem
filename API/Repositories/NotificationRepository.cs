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
        
        // Connection til database. Kunne også skrive den i launchSettings.json?
        public NotificationRepository(ISubGoalRepository subgoalRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _subgoalRepository = subgoalRepository;

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
        public async Task SendNotificationAsync(Notification notification)
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



        // User
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            var user = await _userCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            return user?.Notifications.OrderByDescending(n => n.TimeStamp).ToList() ?? new List<Notification>();
        }
        
        // Subgoals
        public async Task ConfirmMiniGoalAsync(int? userId, string miniGoalName)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);

            var update = Builders<User>.Update.Set("StudentPlan.$[].MiddleGoals.$[middle].MiniGoals.$[mini].Status", true);

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new JsonArrayFilterDefinition<BsonDocument>("{ 'mini.Name': '" + miniGoalName + "' }"),
                new JsonArrayFilterDefinition<BsonDocument>("{ 'middle.MiniGoals': { $elemMatch: { Name: '" + miniGoalName + "' } } }")
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            await _userRepository.ConfirmMiniGoalAsync(userId, miniGoalName);

        }

        
        // Id
        public async Task<int> GetMaxNotificationIdAsync()
        {
            var users = await _userCollection.Find(_ => true).ToListAsync();
            return users.SelectMany(u => u.Notifications).Max(n => (int?)n.NotificationId) ?? 0;
        }

        
    }
}