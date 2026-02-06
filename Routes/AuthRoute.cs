using WinDiet.Enums;
using WinDiet.Models;
using WinDiet.Services;

namespace WinDiet.Routes;

public static class AuthRoute
{
    public static void AuthRoutes(this WebApplication app)
    {
        var routeGroup = app.MapGroup("/auth");

        routeGroup.MapPost("/token", (AuthService service)
            => service.GenerateToken(new User(
                Guid.NewGuid(),
                "John Doe",
                "JohnDoe@teste.com",
                "password",
                Roles.Admin
            )));
    }
}