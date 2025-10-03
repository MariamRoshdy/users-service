using Microsoft.EntityFrameworkCore;
using UsersService.Data;
using UsersService.Dtos;
using System.Security.Cryptography;
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> VerifyPasswordAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return false;
        
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}