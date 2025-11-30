    using FinanceApp.Models;
    using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

    namespace FinanceApp.Data.Services
    {
        public class UserService : IUserService
        {
            private readonly FinanceAppContext _context;
            private readonly IConfiguration _config;
            private readonly PasswordHasher<AppUser> _hasher;

            public UserService(FinanceAppContext context, IConfiguration config)
            {
                _context = context;
                _config = config;
                _hasher = new PasswordHasher<AppUser>();
            }

            public async Task<AppUser> CreateUserAsync(string username, string password, string email, bool emailConfirmed, string refreshToken)
            {

            var user = new AppUser
            {
                Username = username,
                PasswordHash = _hasher.HashPassword(null, password),
                Email = email,
                EmailConfirmed = emailConfirmed,
                RefreshToken = refreshToken
            };

                _context.Users.Add(user);

                try { await _context.SaveChangesAsync(); }
                catch (DbUpdateException ex)
                {
                    if (IsUniqueConstraintViolation(ex))
                    {
                        throw new InvalidOperationException("A user with this email already exists.");
                    }
                    throw;
                }

                return user;
            }
            private bool IsUniqueConstraintViolation(DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx)
                {
                    return sqlEx.Number == 2601 || sqlEx.Number == 2627; // Unique constraint errors
                }
                return false;
            }

            public async Task<AppUser> GetUserByUsernameAsync(string username)
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            }

            public bool VerifyPassword(AppUser user, string password)
            {
                var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
                return result == PasswordVerificationResult.Success;
            }

            public string GenerateJwtToken(AppUser user)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),   
                    new Claim(ClaimTypes.Name, user.Username)                    
                };

                var token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            public async Task<AppUser> GetUserByEmailAsync(string email)
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }

            public async Task UpdateUserAsync(AppUser user)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public Task<AppUser> GetUserByRefreshToken(string refreshToken)
        {
            var user  = _context.Users.FirstOrDefaultAsync(u => u.RefreshToken== refreshToken);
            return user;
        }
    }
    }
