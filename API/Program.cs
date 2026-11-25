using Infrastructure.Data;
using Application.Interfaces;
using Application.Services;
using Application.DTOs;
using Application.Validation;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var passwordHasher = new PasswordHasher<User>();
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddScoped<IHomeService, HomeService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var jwtKey = Convert.FromBase64String(
    builder.Configuration["Jwt:Key"]
    ?? throw new Exception("Brak klucza JWT"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "concert-ticket-manager",

            ValidateAudience = true,
            ValidAudience = "concert-ticket-manager-client",

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Obsługa naszych formularzy HTML/CSS/JS
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

// Rejestracja
app.MapPost("/api/auth/register", async (HttpRequest request, AppDbContext db) =>
{
    var form = await request.ReadFormAsync();

    var user = new RegisterRequest
    {
        FirstName = form["first_name"].ToString(),
        LastName = form["last_name"].ToString(),
        Email = form["email"].ToString(),
        PhoneNumber = form["phone_number"].ToString(),
        Password = form["password"].ToString(),
        ConfirmPassword = form["confirm_password"].ToString()
    };

    var validator = new UserValidator();
    var errors = validator.Validate(user);

    if (errors.Count > 0)
        return Results.UnprocessableEntity(errors);

    var email = user.Email.Trim().ToLowerInvariant();

    if (await db.Users.AnyAsync(u => u.Email == email))
    {
        return Results.UnprocessableEntity(
            new Dictionary<string, string>
            {
                ["email"] = "Użytkownik z takim e-mailem już istnieje."
            });
    }

    var newUser = new User
    {
        UserId = Guid.NewGuid(),
        FirstName = user.FirstName.Trim(),
        LastName = user.LastName.Trim(),
        Email = email,
        PhoneNumber = string.IsNullOrWhiteSpace(user.PhoneNumber)
            ? null : user.PhoneNumber,
        PasswordHash = string.Empty,
        Role = "USER",
        CreatedAt = DateTime.UtcNow
    };

    newUser.PasswordHash = passwordHasher.HashPassword(newUser, user.Password);
    db.Users.Add(newUser);
    await db.SaveChangesAsync();

    return Results.Ok();
});

// Logowanie
app.MapPost("/api/auth/login", async (HttpRequest request, AppDbContext db) =>
{
    var form = await request.ReadFormAsync();

    var email = form["email"].ToString().Trim().ToLowerInvariant();
    var password = form["password"].ToString();

    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

    if (user == null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
    {
        return Results.Json(
            new { error = "Nieprawidłowy e-mail lub hasło." },
            statusCode: 401);
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

        var token = new JwtSecurityToken(
            issuer: "concert-ticket-manager",
            audience: "concert-ticket-manager-client",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(jwtKey),
                SecurityAlgorithms.HmacSha256)
        );

    return Results.Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token)
    });
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
