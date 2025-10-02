using System.Net;
using Microsoft.Extensions.Options;
using WebServiceMonitor.Models;
using WebServiceMonitor.Services;

namespace WebServiceMonitor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient;
    private readonly IEmailService _emailService;
    private readonly MonitoringSettings _settings;
    private bool _lastCheckSuccessful = true;

    public Worker(
        ILogger<Worker> logger,
        IHttpClientFactory httpClientFactory,
        IEmailService emailService,
        IOptions<MonitoringSettings> settings)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _emailService = emailService;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _emailService.SendAlertAsync(
                "Test Email",
                "This is a test email to verify the email notification system is working correctly.");
            _logger.LogInformation("Test email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test email");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var isAvailable = await CheckServiceAvailability();
                await LogStatus(isAvailable);

                if (!isAvailable)
                {
                    _logger.LogWarning("Service is unavailable, attempting to send alert");
                    if (_lastCheckSuccessful)
                    {
                        try
                        {
                            await _emailService.SendAlertAsync(
                                "Service Unavailable Alert",
                                $"The service at {_settings.ServiceUrl} is not responding.");
                            _logger.LogInformation("Alert email sent successfully");
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, "Failed to send alert email");
                        }
                    }
                }

                _lastCheckSuccessful = isAvailable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during service monitoring");
            }

            await Task.Delay(TimeSpan.FromSeconds(_settings.CheckIntervalSeconds), stoppingToken);
        }
    }

    private async Task<bool> CheckServiceAvailability()
    {
        try
        {
            var response = await _httpClient.GetAsync(_settings.ServiceUrl);
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    private async Task LogStatus(bool isAvailable)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var status = isAvailable ? "OK" : "NOK";
        var logEntry = $"[{timestamp}] {status}{Environment.NewLine}";
        
        await File.AppendAllTextAsync(_settings.LogFilePath, logEntry);
    }
}