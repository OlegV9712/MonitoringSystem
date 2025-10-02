using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace WebServiceMonitor.Configuration;

public static class KeyVaultConfiguration
{
    public static IConfigurationBuilder AddCustomAzureKeyVault(this IConfigurationBuilder builder, string keyVaultUri)
    {
        var credential = new DefaultAzureCredential(
            new DefaultAzureCredentialOptions
            {
                ExcludeEnvironmentCredential = false,
                ExcludeManagedIdentityCredential = false,
                ExcludeVisualStudioCredential = true,
                ExcludeAzureCliCredential = true,
                ExcludeInteractiveBrowserCredential = true
            });

        return builder.AddAzureKeyVault(new Uri(keyVaultUri), credential);
    }
}