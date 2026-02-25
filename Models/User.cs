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
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Roles Role { get; set; } = Roles.Client;
    
    public virtual Professional? Professional { get; set; }
    public virtual Patient? Patient { get; set; }
}