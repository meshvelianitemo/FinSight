using FinanceApp.Models;

namespace FinanceApp.Data.Services
{
    public interface IVerificationService
    {
        public Task<EmailVerificationToken> GenerateEmailVerificationTokenAsync(int userId, int validMinutes = 15);
        public Task<bool> ValidateTokenAsync(int userId, string token);
    }
}
