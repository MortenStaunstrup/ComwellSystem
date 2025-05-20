using Core;

public interface IUserRepository
{
    Task AddUserAsync(User user);
    Task<User> Login(string email, string password);
    Task<User?> GetUserByUserId(int userId);
    Task<List<User>?> GetAllKitchenManagersAsync();
    Task<User> GetUserByLoginAsync(int id);
    Task<List<User>> GetAllUsersAsync();
    Task<List<User>> GetAllStudentsAsync();
    Task<int> GetMaxUserId();
    Task<List<User>?> GetAllStudentsByResponsibleIdAsync(int responsibleId);
}