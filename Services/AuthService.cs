using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WinDiet.Common;
using WinDiet.Data;
using WinDiet.DTOs;
using WinDiet.Enums;
using WinDiet.Models;

namespace WinDiet.Services;

public class AuthService(AppDbContext db, IConfiguration configuration)
{
    private readonly string _secretKey = configuration["JwtSettings:SecretKey"]
                                         ?? throw new InvalidOperationException(
                                             "JwtSettings:SecretKey is not configured");

    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyPassword(string password, string hashedPassword) =>
        BCrypt.Net.BCrypt.Verify(password, hashedPassword);

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaimsIdentity(user),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(2),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaimsIdentity(User user)
    {
        var ci = new ClaimsIdentity();
        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Name, user.Name));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        ci.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));

        return ci;
    }

    public async Task<ServiceResult<User>> RegisterProfessional(RegisterProfessionalDTO data)
    {
        if (await db.Users.AnyAsync(u => u.Email == data.Email))
            return ServiceResult<User>.Fail("Esse email já está registrado.");

        if (await db.Professionals.AnyAsync(p => p.CRN == data.CRN))
            return ServiceResult<User>.Fail("Este CRN já está em uso.");

        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var hashedPassword = HashPassword(data.Password);

            var newUser = new User(data.Name, data.Email, hashedPassword);
            newUser.Role = Roles.Professional;

            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            var newProfessional = new Professional(newUser.Id, data.CRN, data.Specialty, data.Bio);

            db.Professionals.Add(newProfessional);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();

            return ServiceResult<User>.Ok(newUser);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<User>.Fail("Erro interno ao registrar profissional: " + e.Message);
        }
    }

    public async Task<ServiceResult<User>> RegisterPatient(RegisterPatientDTO data, Guid userId)
    {
        var professional = await db.Professionals.FirstOrDefaultAsync(p => p.UserId == userId);

        if (professional == null)
            return ServiceResult<User>.Fail("Profissional não encontrado para o usuário logado.");
        
        if (await db.Users.AnyAsync(u => u.Email == data.Email))
            return ServiceResult<User>.Fail("Esse email já está registrado.");

        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var hashedPassword = HashPassword(data.Password);

            var newUser = new User(data.Name, data.Email, hashedPassword);

            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            var newPatient = new Patient(newUser.Id, userId, data.BirthDate, data.Gender, data.Height,
                data.TargetWeight, data.Observations);

            db.Patients.Add(newPatient);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();

            return ServiceResult<User>.Ok(newUser);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<User>.Fail("Erro interno ao registrar profissional: " + e.Message);
        }
    }

    public async Task<ServiceResult<User>> Login(LoginDTO data)
    {
        User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == data.Email);

        if (user == null || !VerifyPassword(data.Password, user.Password))
        {
            return ServiceResult<User>.Fail("Email ou senha inválidos.");
        }

        return ServiceResult<User>.Ok(user);
    }
}