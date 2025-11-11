using System;
using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OrderConsumer.Services;

public class EmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = Environment.GetEnvironmentVariable("OUTLOOK_EMAIL");
        var password = Environment.GetEnvironmentVariable("OUTLOOK_PASSWORD");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Order Consumer", email));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(email, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            Console.WriteLine($"✅ Email sent successfully to {to}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error sending email: {ex.Message}");
        }
    }
}
