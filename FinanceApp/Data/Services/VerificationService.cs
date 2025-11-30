using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace FinanceApp.Data.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly FinanceAppContext _context;

        public VerificationService(FinanceAppContext context)
        {
            _context = context;
        }

        public static string GenerateToken(int length = 8)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            using var rng = RandomNumberGenerator.Create();

            var data = new byte[length];
            rng.GetBytes(data);

            char[] result = new char[length];
            for (int i = 0; i < length; i++)
                result[i] = chars[data[i] % chars.Length];

            return new string(result);
        }

        public async Task<EmailVerificationToken> GenerateEmailVerificationTokenAsync(int userId, int validMinutes = 15)
        {
            var token = new EmailVerificationToken
            {
                UserId = userId,
                Token = GenerateToken(6),
                ExpiresAt = DateTime.Now.AddMinutes(validMinutes),
                Used = false
            };

            _context.EmailVerificationTokens.Add(token);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<bool> ValidateTokenAsync(int userId, string token)
        {
            var dbToken = await _context.EmailVerificationTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token && !t.Used && t.ExpiresAt > DateTime.UtcNow);

            if (dbToken == null) return false;

            dbToken.Used = true;
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
