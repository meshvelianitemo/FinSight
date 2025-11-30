using FinanceApp.Models;

namespace FinanceApp.Data.Services
{
    public interface IUserService
    {
        Task<AppUser> CreateUserAsync(string username, string password, string email, bool emailConfirmed, string refreshToken);
        Task<AppUser> GetUserByUsernameAsync(string username);
        bool VerifyPassword(AppUser user, string password);
        string GenerateJwtToken(AppUser user);
        Task<AppUser> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(AppUser user);
        string GenerateRefreshToken();
        Task<AppUser> GetUserByRefreshToken(string refreshToken);

    }
}
