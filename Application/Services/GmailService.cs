using System.Net.Mail;
using Application.helpers;
using Application.IServices;
using MailKit.Net.Smtp;
using MailKit;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Application.Services
{
    public class GmailEmailService : IEmailService
    {
        private const string SmtpServer = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string SmtpUsername = "WhereHouseIncBusiness@gmail.com"; // Replace with your Gmail address
        private readonly string SmtpPassword; // SMTP password field

        public GmailEmailService(IOptions<AppSettings> settings)
        {
            SmtpPassword = settings.Value.SMTP_KEY; // Access SMTP_PASSWORD
        }
        
        public void SendTemporaryCredentials(string receiverEmail, string password)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("WhereHouse Inc",SmtpUsername));
            email.To.Add(new MailboxAddress("Employee",receiverEmail));

            email.Subject = "Temporary access password";

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { 
                Text = $"<b>This is your temporary access password, which you should change asap {password}</b>"
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(SmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);


                smtp.Authenticate(SmtpUsername,SmtpPassword);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }

        public void ContactSupport(string contactEmail, string description)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("WhereHouse Inc", SmtpUsername)); // Replace with your details
            email.ReplyTo.Add(new MailboxAddress("User",contactEmail)); // User's email'));
            email.To.Add(new MailboxAddress("Support", SmtpUsername)); // Your support email

            email.Subject = "New Support Request";

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { 
                Text = $"<p><b>User Email:</b> {contactEmail}</p><p><b>Description:</b> {description}</p>"
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(SmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(SmtpUsername, SmtpPassword);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }

    }
}