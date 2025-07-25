using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;


namespace Software_Solutions.Services
{
    public class EmailServices
    {
        private readonly IConfiguration _config;

        public EmailServices(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var email = new MimeMessage();// Email message

            email.From.Add(new MailboxAddress(_config["MailSettings:SenderName"],_config["MailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlContent };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["MailSettings:SmtpHost"], 587 , SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["MailSettings:UserName"], _config["MailSettings:SmtpPassword"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
