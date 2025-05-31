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

    // login endpointv: returnerer bruger hvis email og password matcher, ellers unauthorized
    // param email: brugerens email
    // password: brugerens adgangskode
    // return: brugerobjekt uden adgangskode hvis godkendt, ellers 401
    [HttpGet("login/{email}/{password}")]  // Endpoint til login med e-mail og password
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _repo.Login(email, password);  // Tjekker om brugeren findes
        if (user == null)
        {
            return Unauthorized();  // Hvis ikke, send et eller andet svar. Lige nu sender den 404. Burde nok bare sende en fejlbesked. Rettes.
        }

        user.UserPassword = ""; //mere simpelt end at skrive alle ting den skal tage med
        return Ok(user);
        {
        }
    }

    // opretter en ny bruger hvis bruger-id ikke findes i forvejen
    // param user: det brugerobjekt der skal oprettes
    // return: den oprettede bruger eller null hvis bruger allerede findes
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
    
    [HttpGet("student")]  // Henter alle brugere fra databasen med rollen student
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
    
    // henter alle elever som en bestemt leder har ansvar for
    // param responsibleId: bruger-id på den ansvarlige leder
    // return: liste over tilknyttede elever (elever som har lederens userId som userIdResponsible)
    [HttpGet("students/{responsibleId:int}")]
    public async Task<List<User>?> GetAllStudentsByResponsibleIdAsync(int responsibleId)
    {
        return await _repo.GetAllStudentsByResponsibleIdAsync(responsibleId);
    }
    
    // henter alle brugere med rollen kitchenmanager
    // return: liste over køkkenansvarlige
    [HttpGet("kitchenmanagers")]
    public async Task<List<User>?> GetAllKitchenManagersAsync()
    {
        return await _repo.GetAllKitchenManagersAsync();
    }
    
    
    // opdaterer en bruger i databasen
    // param updatedUser: det opdaterede brugerobjekt
    // return: 400 hvis objektet er null, ellers 200
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] User updatedUser)
    {
        Console.WriteLine("Updating User :Controller");
        if (updatedUser == null)
            return BadRequest("Bruger er null");

        await _repo.UpdateUserAsync(updatedUser);
        return Ok();
    }
    
    // sletter en bruger fra databasen
    // param userId: id på bruger der skal slettes
    // return: 404 hvis bruger ikke findes, ellers 204
    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteUser(int userId) //IActionResult så vi har flere returtyper
    {
        var existingUser = await _repo.GetUserByUserId(userId);
        if (existingUser == null)
        {
            return NotFound();
        }

        await _repo.DeleteUserAsync(userId);
        return NoContent();
    }
}