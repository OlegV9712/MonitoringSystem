namespace WebServiceMonitor.Models;

public class MonitoringSettings
{
    public required string ServiceUrl { get; set; }
    public int CheckIntervalSeconds { get; set; } = 60;
    public required string LogFilePath { get; set; }
    public required EmailSettings Email { get; set; }
}

public class EmailSettings
{
    public required string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FromAddress { get; set; }
    public required string ToAddress { get; set; }
    public bool UseSSL { get; set; }
}