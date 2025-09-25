using Microsoft.EntityFrameworkCore;
using UsersService.Data;
using UsersService.Dtos;
using System.Security.Cryptography;
using System.Text;
using UsersService.Models;
using System.Text.RegularExpressions;

namespace UsersService.ApplicationServices;

public class AuthService
{
    private readonly UsersDBContext _context;
    public AuthService(UsersDBContext context)
    {
        _context = context;
    }
    public async Task RegisterUserAsync(RegisterDto dto)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("Email already in use.");

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[A-Za-z]+$");
        if (!emailRegex.IsMatch(dto.Email))
            throw new Exception("Invalid email format.");

        // Hash the password
        using var hmac = new HMACSHA256();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)))
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}