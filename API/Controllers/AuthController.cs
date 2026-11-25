using Application.DTOs;
using Application.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    AppDbContext db,
    AuthService authService,
    SymmetricSecurityKey signingKey) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register()
    {
        var form = await Request.ReadFormAsync();

        var user = new RegisterRequest
        {
            FirstName = form["first_name"].ToString(),
            LastName = form["last_name"].ToString(),
            Email = form["email"].ToString(),
            PhoneNumber = form["phone_number"].ToString(),
            Password = form["password"].ToString(),
            ConfirmPassword = form["confirm_password"].ToString()
        };

        var errors = authService.ValidateRegistration(user);

        if (errors.Count > 0)
            return UnprocessableEntity(errors);

        var email = authService.NormalizeEmail(user.Email);

        if (await db.Users.AnyAsync(u => u.Email == email))
        {
            return UnprocessableEntity(
                new Dictionary<string, string>
                {
                    ["email"] = "Użytkownik z takim e-mailem już istnieje."
                });
        }

        var newUser = authService.CreateUser(user);

        db.Users.Add(newUser);
        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        var form = await Request.ReadFormAsync();

        var email = authService.NormalizeEmail(
            form["email"].ToString());

        var password = form["password"].ToString();

        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Email == email);

        if (user == null || !authService.VerifyPassword(user, password))
        {
            return Unauthorized(
                new { error = "Nieprawidłowy e-mail lub hasło." });
        }

        var token = new JwtSecurityToken(
            issuer: "concert-ticket-manager",
            audience: "concert-ticket-manager-client",
            claims: authService.CreateClaims(user),
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}
