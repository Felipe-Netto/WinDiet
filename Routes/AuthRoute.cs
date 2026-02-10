using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WinDiet.Data;
using WinDiet.DTOs;
using WinDiet.Services;

namespace WinDiet.Routes;

public static class AuthRoute
{
    public static void AuthRoutes(this WebApplication app)
    {
        var routeGroup = app.MapGroup("/auth");

        routeGroup.MapPost("/register/professional",
            async (AppDbContext db, AuthService authService, RegisterDTO data) =>
            {
                var result = await authService.RegisterProfessional(data, db);

                if (!result.Success)
                    return Results.Conflict(new { message = result.ErrorMessage });

                var token = authService.GenerateToken(result.Data!);
                return Results.Ok(new { token });
            });

        routeGroup.MapPost("/login", async (AppDbContext db, AuthService authService, LoginDTO data) =>
        {
            var result = await authService.Login(data, db);
            
            if (!result.Success)
                return Results.Unauthorized();
            
            var token = authService.GenerateToken(result.Data!);
            return Results.Ok(new { token });
        });

        routeGroup.MapGet("/me", (ClaimsPrincipal user) =>
            {
                var userData = new
                {
                    Id = user.FindFirstValue(ClaimTypes.NameIdentifier),
                    Name = user.FindFirstValue(ClaimTypes.Name),
                    Email = user.FindFirstValue(ClaimTypes.Email),
                    Role = user.FindFirstValue(ClaimTypes.Role)
                };

                return Results.Ok(userData);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin"});
    }
}