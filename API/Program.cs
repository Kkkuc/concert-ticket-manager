using Infrastructure.Data;
using Application.Interfaces;
using Application.Services;
using Application.DTOs;
using Application.Validation;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Istniejąca konfiguracja zespołu
builder.Services.AddControllers();
builder.Services.AddScoped<IHomeService, HomeService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
        PasswordHash = user.Password,
        Role = "USER",
        CreatedAt = DateTime.UtcNow
    };

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

    if (user == null || user.PasswordHash != password)
    {
        return Results.Json(
            new { error = "Nieprawidłowy e-mail lub hasło." },
            statusCode: 401);
    }

    return Results.Ok();
});

// Zachowujemy kontrolery zespołu
app.MapControllers();

app.Run();
