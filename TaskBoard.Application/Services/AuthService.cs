using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskBoard.Application.Dtos;
using TaskBoard.Application.Interfaces;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly TaskBoardDbContext _db;
        private readonly PasswordHasher<User> _hasher = new();
        private readonly IConfiguration _cfg;

        public AuthService(TaskBoardDbContext db, IConfiguration cfg)
        {
            _db = db;
            _cfg = cfg;
        }

        public Guid Register(RegisterDto dto)
        {
            if (_db.Users.Any(u => u.Email == dto.Email))
                throw new InvalidOperationException("User already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = _hasher.HashPassword(null!, dto.Password)
            };

            _db.Users.Add(user);
            _db.SaveChanges();
            return user.Id;
        }

        public string Login(LoginDto dto)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == dto.Email)
                       ?? throw new InvalidOperationException("Invalid credentials");

            var result = _hasher.VerifyHashedPassword(null!, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Invalid credentials");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
