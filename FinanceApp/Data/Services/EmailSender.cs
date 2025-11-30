
using System.Net;
using System.Net.Mail;

namespace FinanceApp.Data.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mail = "appfinance49@gmail.com";
            var pw = "eyut ruke kees bgzj";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(mail, pw),
                EnableSsl = true
            };

            return client.SendMailAsync(
                new MailMessage(from : mail, to : toEmail, subject, message)
                {
                    IsBodyHtml = true
                });
        }
    }
}
