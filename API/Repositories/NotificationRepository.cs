using API.Repositories.Interface;
using Core;
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
        public async Task ConfirmNotifiedSubgoalAsync(int notificationId)
        {
            var filter = Builders<User>.Filter.ElemMatch(u => u.Notifications, n => n.NotificationId == notificationId);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("Notification not found");

            var notification = user.Notifications.First(n => n.NotificationId == notificationId);

            // Opdater minigoal i user via IUserRepository (du skal tilføje metoden der)
            await _userRepository.ConfirmMiniGoalAsync(notification.UserId, notification.MiniGoalName);

            // Fjern notifikationen fra embedded listen
            var update = Builders<User>.Update.PullFilter(u => u.Notifications, n => n.NotificationId == notificationId);
            await _userCollection.UpdateOneAsync(u => u.UserId == notification.UserId, update);
        }
        
        // Id
        public async Task<int> GetMaxNotificationIdAsync()
        {
            var users = await _userCollection.Find(_ => true).ToListAsync();
            return users.SelectMany(u => u.Notifications).Max(n => (int?)n.NotificationId) ?? 0;
        }

        
    }
}