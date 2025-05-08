namespace Core;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Role { get; set;}
    public string UserPassword { get; set; }
    public string UserEmail { get; set; }
    public string UserPhone { get; set; }
    public DateOnly? UserSemester { get; set; }
    public int? UserIdResponsible { get; set; }
}