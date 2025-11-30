using System.ComponentModel.DataAnnotations;


namespace FinanceApp.Models
{
    public class EmailVerificationToken
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public AppUser User { get; set; }
        [Required]
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; } = false;
}
}
