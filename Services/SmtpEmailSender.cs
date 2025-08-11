using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace JabbadabbadoeBooking.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailSender> _logger;
    public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
    {
        _config = config; _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var fromName = _config["Email:FromName"] ?? "Jabbadabbadoe";
        var fromAddr = _config["Email:FromAddress"] ?? "no-reply@example.com";
        var host = _config["Email:SmtpHost"];
        var port = int.TryParse(_config["Email:SmtpPort"], out var p) ? p : 587;
        var useStartTls = bool.TryParse(_config["Email:UseStartTls"], out var tls) ? tls : true;
        var username = _config["Email:Username"];
        var password = _config["Email:Password"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogInformation("SMTP not configured; logging email to console. To: {to}, Subject: {subject}", toEmail, subject);
            Console.WriteLine("=== EMAIL (FAKE) ===");
            Console.WriteLine($"To: {toEmail}\nSubject: {subject}\n{htmlBody}");
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromAddr));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
        await client.AuthenticateAsync(username, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}