using API.Repositories.Interface;
using Core;
using MongoDB.Driver;

namespace API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly string _connectionString = "mongodb+srv://Hjalte:Hjalte123@clusterfree.a2y2b.mongodb.net/";
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Notification> _collection;

        public NotificationRepository()
        {
            _client = new MongoClient(_connectionString);
            _database = _client.GetDatabase("Comwell");
            _collection = _database.GetCollection<Notification>("Notifications");
        } 

        public async Task AddNotificationAsync(Notification notification) //elev sender en notifikation til køkkenleder
        {
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsConfirmed = false; //IsConfirmed bruges af køkkenleder
            await _collection.InsertOneAsync(notification);
        }

        public async Task<List<Notification>> GetNotificationsForUserAsync(int userId) //Køkkenleder modtager notifikationen
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.ReceiverUserId, userId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task ConfirmNotificationAsync(int notificationId) //køkkenleder bekræfter.
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.NotificationId, notificationId);
            var update = Builders<Notification>.Update.Set(n => n.IsConfirmed, true);
            await _collection.UpdateOneAsync(filter, update);
        }
        
    }
}