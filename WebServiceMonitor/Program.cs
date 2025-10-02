using WebServiceMonitor;
using WebServiceMonitor.Models;
using WebServiceMonitor.Services;
using WebServiceMonitor.Configuration;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["KeyVault:Uri"];
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddCustomAzureKeyVault(keyVaultUri);
    }
}
builder.Services.Configure<MonitoringSettings>(
    builder.Configuration.GetSection(nameof(MonitoringSettings)));

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();