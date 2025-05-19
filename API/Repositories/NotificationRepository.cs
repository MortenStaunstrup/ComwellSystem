using API.Repositories.Interface;
using Core;
using MongoDB.Driver;

namespace API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {   private readonly string _connectionString;
        private readonly MongoClient _client;
        private readonly IMongoCollection<Notification> _collection;

        public NotificationRepository()
        {
            _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("No connection string configured");
            }
            _client = new MongoClient(_connectionString);
           
            var db = _client.GetDatabase("Comwell");
            _collection = db.GetCollection<Notification>("Notifications");
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsConfirmed = false;
            await _collection.InsertOneAsync(notification);
        }
        public async Task ConfirmNotifiedSubgoalAsync(int notificationId)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.Id, notificationId);
            var update = Builders<Notification>.Update.Set(n => n.IsConfirmed, true);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<int> GetMaxNotificationIdAsync()
        {
            var sort = Builders<Notification>.Sort.Descending(n => n.Id);
            var maxNotification = await _collection.Find(Builders<Notification>.Filter.Empty).Sort(sort).Limit(1).FirstOrDefaultAsync();
            return maxNotification?.Id ?? 0;
        }
        public async Task<List<Notification>> GetNotificationsForUserAsync(int userId)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.ReceiverUserId, userId);

            var notifications = await _collection
                .Find(filter)
                .SortByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications ?? new List<Notification>();
        }


    }
}