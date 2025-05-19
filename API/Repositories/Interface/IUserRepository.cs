using Core;

// Interface til repository, som snakker med databasen
public interface IUserRepository
{
    Task AddUserAsync(User user);  // Tilføj en ny bruger

    Task<User> Login(string email, string password);  // Find bruger ud fra email og password
    Task<User?> GetUserByUserId(int userId);

    Task<User> GetUserByLoginAsync(int id);  // Find bruger ud fra bruger-id

    Task<List<User>> GetAllUsersAsync();  // Hent alle brugere

    Task<List<User>> GetAllStudentsAsync();
    Task<List<User>?> GetAllStudentsByResponsibleIdAsync(int responsibleId);

    Task<int> GetMaxUserId();  // Hent den højeste bruger-id i databasen
    
    Task EmbedNotificationToUserAsync(User user, Notification notification);
}