using WinDiet.Enums;

namespace WinDiet.Models;

public record User(Guid Id, string Name, string Email, string Password, Roles Role);