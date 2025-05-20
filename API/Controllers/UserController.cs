using Core;
using Core.Models;
using Microsoft.AspNetCore.Mvc;



[ApiController] // Bare for at gøre HTTP-requests mulige: fx GET og POST
[Route("api/[controller]")] // URL bliver fx. api/users
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo; // Så vi kan hente fra databasen

    public UsersController(IUserRepository repository)
    {
        _repo = repository; // Repository sendes ind og gemmes i en variabel
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) //så password ikke kommer med
    {
        var user = await _repo.Login(request.Email, request.Password);
        if (user == null)
            return Unauthorized("Forkert email eller adgangskode.");
    
        return Ok(new
        {
            user.UserId,
            user.UserEmail,
            user.Role,
            user.UserIdResponsible
        });
    }
    
    [HttpPost("Opret")]
    public async Task<User?> AddUserAsync(User user)
    {
        var existingUser = await _repo.GetUserByLoginAsync(user.UserId); // Tjekker om brugeren allerede findes
        if (existingUser != null)
        {
            return null;
        }

        _repo.AddUserAsync(user); // Tilføjer ny bruger

        return user;
    }

    [HttpGet] // Henter alle brugere fra databasen
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _repo.GetAllUsersAsync();
    }

    [HttpGet("student")] // Henter alle brugere fra databasen
    public async Task<List<User>> GetAllStudentsAsync()
    {
        return await _repo.GetAllStudentsAsync();
    }

    [HttpGet("user/{userId:int}")]
    public async Task<User?> GetUserByUserId(int userId)
    {
        Console.WriteLine($"Returning user: {userId}: controller");
        return await _repo.GetUserByUserId(userId);
    }

    [HttpGet("maxid")] // Henter den højeste bruger-id fra databasen
    public async Task<int> GetMaxUserId()
    {
        return await _repo.GetMaxUserId();
    }
    [HttpGet("students/{responsibleId:int}")]
    public async Task<List<User>?> GetAllStudentsByResponsibleIdAsync(int responsibleId)
    {
        return await _repo.GetAllStudentsByResponsibleIdAsync(responsibleId);
    }

    [HttpGet("kitchenmanagers")]
    public async Task<List<User>?> GetAllKitchenManagersAsync()
    {
        return await _repo.GetAllKitchenManagersAsync();
    }
}

/* [HttpPost("{userId}/add-notification")]
 public async Task<IActionResult> AddNotificationToUser(int userId, [FromBody] Notification notification) //frombody fordi metoden skal bruge inforamtion fra andet end bare http
 {
     var user = await _repo.GetUserByLoginAsync(userId);
     if (user == null)
         return NotFound("User not found");

     await _repo.EmbedNotificationToUserAsync(user, notification);
     return Ok();
 
    }*/
    