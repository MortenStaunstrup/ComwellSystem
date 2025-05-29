using API.Repositories.Interface;
using Core;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {   
        private readonly string _connectionString;
        
        private readonly MongoClient _client;
        private readonly IMongoCollection<User> _userCollection;
        
        public NotificationRepository()
        {

            // Connection til database. Kunne også skrive den i launchSettings.json? Repository fungere bare ligesom vores mongodb compass. Vi skal bruge en connectionstring med username og password. 
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
        
        
        //tilføjer en notifikation til en bruger, når et miniGoal markeres som færdigt af en student.
        public async Task SendMiniGoalNotificationAsync(Notification notification)
        {
            notification.NotificationId = await GetMaxNotificationIdAsync() + 1; // genererer et nyt ID som er +1 for hver notifikation
            notification.TimeStamp = DateTime.UtcNow; // hvornår noti bliver sendt

            var filter = Builders<User>.Filter.Eq(u => u.UserId, notification.UserId); // mongoDB filter til at finde en specefik user. 
            var update = Builders<User>.Update.Push(u => u.Notifications, notification); // når brugeren er fundet skubber vi notifikationen ind i user-objektet (embedding)

            var result = await _userCollection.UpdateOneAsync(filter, update); // opdaterer userobjektet

            if (result.MatchedCount == 0) // debugging: matchedcount fortæller os om vi kan finde useren i databasen.
            {
                throw new Exception($"Ingen bruger med UserId {notification.UserId} fundet");
            }

            if (result.ModifiedCount == 0) // debugging: modifiedcount fortæller os, om vi har ændret noget i databasen.
            {
                throw new Exception($"Notification blev ikke tilføjet til bruger {notification.UserId}");
            }
            
        }
        
        public async Task SendMiddleGoalNotificationAsync(Notification notification) // samme som for minigoal overstående
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
        
        // opdaterer status for et specifikt MiniGoal til true, når det bliver bekræftet af en kitchenmanager
        public async Task<bool> UpdateMiniGoalStatusAsync(int studentId, string miniGoalName) // modtager en eleves userId som er studentId, og bruger primærnøglen name i minigoals til at opdatere det.
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, studentId); // find først brugeren med det tilsvarende userId i databasen

            var update = Builders<User>.Update
                .Set("StudentPlan.$[plan].MiddleGoals.$[middle].MiniGoals.$[mini].Status", true); // update-statement, som sætter status på det rigtige MiniGoal til true. den går ind i studentplan - middlegoals - minigoals - status for minigoal

            var arrayFilters = new List<ArrayFilterDefinition> // så databasen kan finde det rigtige minigoal.
            {   
                //pakker bsondokumenterne ind i objekter.
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("mini.Name", miniGoalName)), // sammenligner minigoal name med vores parameter hele vejen igennem embeddingen, for at være sikker. Update skal kun gælde for det rigtige minigoal.
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("middle.MiniGoals.Name", miniGoalName)),
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("plan.MiddleGoals.MiniGoals.Name", miniGoalName))
            };

            var options = new UpdateOptions { ArrayFilters = arrayFilters };

            var result = await _userCollection.UpdateOneAsync(filter, update, options); //updater med de variable vi har sat
            return result.ModifiedCount > 0;
        }

        // Fjerner en minigoal-notifikation fra en leder (kitchenmanager), når notifikationen er blevet bekræftet.
        
        //leaderId: userId for kitchenmanager. leader her, men det er det samme som kitchenmanager. Lidt inkonsistent
        // notificationId: ID på den notifikation, der skal fjernes.
        public async Task<bool> RemoveNotificationMiniGoalFromManagerAsync(int leaderId, int notificationId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, leaderId); // finder kitchenmanagers user-objekt gennem deres userId.

            var update = Builders<User>.Update
                .PullFilter(u => u.Notifications, n => n.NotificationId == notificationId); // fjerner (pull) notifikation fra listen, hvor notificationId matcher.

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }


        public async Task<bool> UpdateMiddleGoalStatusAsync(int studentId, string middleGoalName) //samme som minigoal nogenlunde
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