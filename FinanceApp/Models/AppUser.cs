using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanceApp.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class AppUser

    {

        public int Id { get; set; }

        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }

        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
