using Ass1.ViewModels;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Ass1.Utils
{
    public class MailUtils
    {
        private readonly string username = "thecoursearnteam@gmail.com";
        private readonly string password = "zgwf djfl ylcg owww"; // Use a secure method to store and retrieve your password
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int port = 587;

        public async Task SendMailAsync(string fromEmail, string fromName, string toEmail, string toName,
                                        string subject, string body, string replyTo = "")
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            if (!string.IsNullOrEmpty(replyTo))
            {
                message.From.Add(new MailboxAddress(string.Empty, fromEmail));
                message.To.Add(new MailboxAddress(string.Empty, toEmail));
            }

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(smtpServer, port, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(username, password);
                    await client.SendAsync(message);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }

        public async Task NotificationMailAsync(NewsArticleViewModel model, string adminEmail)
        {
            string subject = $"New Article Published: {model.NewsTitle}";
            string body = $@"
                <h1>{model.NewsTitle}</h1>
                <h2>{model.Headline}</h2>
                <p><strong>Source:</strong> {model.NewsSource}</p>
                <p><strong>Published On:</strong> {model.CreatedDate?.ToString("f")}</p>
                <hr>
                <div>{model.NewsContent}</div>";

            string fromEmail = username;
            string fromName = "News Portal";
            string toName = "Admin";

            await SendMailAsync(fromEmail, fromName, adminEmail, toName, subject, body);
        }
    }
}