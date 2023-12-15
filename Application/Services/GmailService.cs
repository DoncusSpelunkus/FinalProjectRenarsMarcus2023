using System.Net.Mail;
using Application.IServices;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Application.Services
{
    public class GmailEmailService : IEmailService
    {
        private const string SmtpServer = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string SmtpUsername = "WhereHouseIncBusiness@gmail.com"; // Replace with your Gmail address
        private const string SmtpPassword = "qwce jypp xwqx xicw"; // Replace with your Gmail password

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
    }
}