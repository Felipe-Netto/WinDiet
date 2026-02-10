using WinDiet.Enums;

namespace WinDiet.Models;

public class User 
{
    
    public User(String name, String email, String password)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = password;
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Roles Role { get; set; } = Roles.User;
}