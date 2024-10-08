using MegStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace MegStore.Application.Services
{
    public class EmailService : IEmailService
    {
        // Updated SMTP server settings for Zoho Mail
        private readonly string _smtpServer = "smtp.zoho.com";
        private readonly int _smtpPort = 587; // Port for TLS
        private readonly string _username;
        private readonly string _password;

        public EmailService(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("MEGSTORE", _username));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("html")
            {
                Text = $"<strong>{message}</strong>"
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // Connect to Zoho Mail SMTP server
                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_username, _password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to send email", ex);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }

        public Task SendEmailsAsyncList(List<string> emails, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}
