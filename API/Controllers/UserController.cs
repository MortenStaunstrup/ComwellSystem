using Core;
using Microsoft.AspNetCore.Mvc;



[ApiController]  // Bare for at gøre HTTP-requests mulige: fx GET og POST
[Route("api/[controller]")]  // URL bliver fx. api/users
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;  // Så vi kan hente fra databasen

    public UsersController(IUserRepository repository)
    {
        _repo = repository;  // Repository sendes ind og gemmes i en variabel
    }

    [HttpGet("login/{email}/{password}")]  // Endpoint til login med e-mail og password
    public async Task<IActionResult> Login(string email, string password) // (bruger iactionresult, så den kan returne forskelligt)
    {
        var user = await _repo.Login(email, password);  // Tjekker om brugeren findes
        if (user == null)
        {
            return Unauthorized("Forkert email eller adgangskode... prøv igen ");  // Hvis ikke, send et eller andet svar.
        }

        return Ok(new User
        {
            UserId = user.UserId,
            UserEmail = user.UserEmail,
            Role = user.Role,
            UserIdResponsible = user.UserIdResponsible,
        });
    }

    [HttpPost("Opret")]
    public async Task<User?> AddUserAsync(User user)
    {
        var existingUser = await _repo.GetUserByLoginAsync(user.UserId);  // Tjekker om brugeren allerede findes
        if (existingUser != null)
        {
            return null;
        }

        _repo.AddUserAsync(user);  // Tilføjer ny bruger

        return user;
    }
    [HttpGet]  // Henter alle brugere fra databasen
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _repo.GetAllUsersAsync();
    }
    
    [HttpGet("student")]  // Henter alle brugere fra databasen
    public async Task<List<User>> GetAllStudentsAsync()
    {
        return await _repo.GetAllStudentsAsync();
    }

    [HttpGet("maxid")]  // Henter den højeste bruger-id fra databasen
    public async Task<int> GetMaxUserId()
    {
        return await _repo.GetMaxUserId();
    }
    [HttpPost("{userId}/add-notification")]
    public async Task<IActionResult> AddNotificationToUser(int userId, [FromBody] Notification notification) //frombody fordi metoden skal bruge inforamtion fra andet end bare http
    {
        var user = await _repo.GetUserByLoginAsync(userId);
        if (user == null)
            return NotFound("User not found");

        await _repo.EmbedNotificationToUserAsync(user, notification);
        return Ok();
    }
    
}