using Application.DTOs;
using Application.Validation;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.Services;

public class AuthService
{
    private readonly UserValidator _validator = new();
    private readonly PasswordHasher<User> _passwordHasher = new();

    public Dictionary<string, string> ValidateRegistration(RegisterRequest user)
    {
        return _validator.Validate(user);
    }

    public string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    public User CreateUser(RegisterRequest request)
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = NormalizeEmail(request.Email),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null : request.PhoneNumber,
            PasswordHash = string.Empty,
            Role = "USER",
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, request.Password);

        return user;
    }

    public bool VerifyPassword(User user, string password)
    {
        return _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password
        ) != PasswordVerificationResult.Failed;
    }

    public Claim[] CreateClaims(User user)
    {
        return new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
    }
}
