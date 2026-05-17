using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.API.Data;
using TaskFlow.API.DTOs;
using TaskFlow.API.Interfaces;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    private readonly IConfiguration _configuration;

    private readonly PasswordHasher<AppUser> _passwordHasher;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;

        _configuration = configuration;

        _passwordHasher = new PasswordHasher<AppUser>();
    }

    public async Task<string?> RegisterAsync(RegisterDto registerDto)
    {
        bool emailExists = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);

        if (emailExists)
        {
            return null;
        }

        AppUser user = new AppUser
        {
            Username = registerDto.Username,
            Email = registerDto.Email
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return GenerateJwtToken(user);
    }

    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        AppUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null)
        {
            return null;
        }

        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(AppUser user)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new Claim(ClaimTypes.Email, user.Email),

            new Claim(ClaimTypes.Name, user.Username),

            new Claim(ClaimTypes.Role, user.Role)
        ];

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
