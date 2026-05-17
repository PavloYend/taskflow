namespace TaskFlow.API.Models;

public class AppUser
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public List<TaskItem> Tasks { get; set; } = [];

    public string Role { get; set; } = "User";
}
