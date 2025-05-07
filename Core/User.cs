namespace Core;

public class User
{
    private int UserId { get; set; }
    private string UserName { get; set;}
    private string Role { get; set;}
    private string UserPassword { get; set; }
    private string UserEmail { get; set; }
    private string UserPhone { get; set; }
    private DateOnly? UserSemester { get; set; }
    private int? UserIdResponsible { get; set; }
}