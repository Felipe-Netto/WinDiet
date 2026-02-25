using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WinDiet.Data;
using WinDiet.DTOs;
using WinDiet.Enums;
using WinDiet.Services;

namespace WinDiet.Routes;

public static class AuthRoute
{
    public static void AuthRoutes(this WebApplication app)
    {
        var routeGroup = app.MapGroup("/auth");

        routeGroup.MapPost("/register/professional",
            async (AuthService authService, RegisterProfessionalDTO data) =>
            {
                var result = await authService.RegisterProfessional(data);

                if (!result.Success)
                    return Results.Conflict(new { message = result.ErrorMessage });

                var token = authService.GenerateToken(result.Data!);
                return Results.Ok(new { token });
            });

        routeGroup.MapPost("/register/patient",
                async (AuthService authService, ClaimsPrincipal user, RegisterPatientDTO data) =>
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    
                    if (string.IsNullOrEmpty(userIdClaim)) 
                        return Results.Unauthorized();
                    
                    var loggedInUserId = Guid.Parse(userIdClaim);
                    
                    var result = await authService.RegisterPatient(data, loggedInUserId);

                    if (!result.Success)
                        return Results.Conflict(new { message = result.ErrorMessage });

                    var token = authService.GenerateToken(result.Data!);
                    return Results.Ok(new { token });
                })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Professional" });

        routeGroup.MapPost("/login", async (AuthService authService, LoginDTO data) =>
        {
            var result = await authService.Login(data);

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
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}