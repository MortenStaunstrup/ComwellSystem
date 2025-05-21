using Core;
using Microsoft.AspNetCore.Mvc;
using API.Repositories.Interface;
using API.Repositories;



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
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _repo.Login(email, password);  // Tjekker om brugeren findes
        if (user == null)
        {
            return Unauthorized();  // Hvis ikke, send et eller andet svar. Lige nu sender den 404. Burde nok bare sende en fejlbesked. Rettes.
        }

        return Ok(new User
        {
            UserId = user.UserId,
            UserEmail = user.UserEmail,
            Role = user.Role
        });
    }

    [HttpPost("Opret")]  // Endpoint til at oprette ny bruger
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

    [HttpGet("user/{userId:int}")]
    public async Task<User?> GetUserByUserId(int userId)
    {
        Console.WriteLine($"Returning user: {userId}: controller");
        return await _repo.GetUserByUserId(userId);
    }

    [HttpGet("maxid")]  // Henter den højeste bruger-id fra databasen
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
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] User updatedUser)
    {
        if (updatedUser == null)
            return BadRequest("Bruger er null");

        await _repo.UpdateUserAsync(updatedUser);
        return Ok();
    }
}