using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using WebServiceMonitor.Models;

namespace WebServiceMonitor.Services;

public interface IEmailService
{
    Task SendAlertAsync(string subject, string message);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<MonitoringSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value.Email;
        _logger = logger;
    }

    public async Task SendAlertAsync(string subject, string message)
    {
        try
        {
            _logger.LogInformation("Attempting to send email alert");
            
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Web Service Monitor", _settings.FromAddress));
            email.To.Add(new MailboxAddress("Administrator", _settings.ToAddress));
            email.Subject = $"[Web Monitor] {subject}";
            email.Body = new TextPart("plain") 
            { 
                Text = $"Monitoring Alert\n\nTimestamp: {DateTime.Now}\n{message}\n\nThis is an automated message from Web Service Monitor." 
            };

            _logger.LogInformation("Connecting to SMTP server: {Server}:{Port}", _settings.SmtpServer, _settings.SmtpPort);
            
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            
            _logger.LogInformation("Authenticating with username: {Username}", _settings.Username);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            
            _logger.LogInformation("Sending email");
            await client.SendAsync(email);
            
            _logger.LogInformation("Disconnecting from SMTP server");
            await client.DisconnectAsync(true);
            
            _logger.LogInformation("Email alert sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email alert");
            throw;
        }
    }
}