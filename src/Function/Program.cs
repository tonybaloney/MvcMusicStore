using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()

.ConfigureServices(services =>
{
    Uri openaiEndPoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") is string value &&
        Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) &&
        uri is not null
        ? uri
        : throw new ArgumentException(
        $"Unable to parse endpoint URI");

    string? apiKey;
    var keyVaultEndpoint = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_ENDPOINT");
    if (!string.IsNullOrEmpty(keyVaultEndpoint))
    {
        var openAIKeyName = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");
        var keyVaultClient = new SecretClient(vaultUri: new Uri(keyVaultEndpoint), credential: new DefaultAzureCredential());
        apiKey = keyVaultClient.GetSecret(openAIKeyName).Value.Value;
    }
    else
    {
        apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");
    }

    var openAIClient = apiKey != null ?
        new AzureOpenAIClient(openaiEndPoint, new AzureKeyCredential(apiKey)) :
        new AzureOpenAIClient(openaiEndPoint, new DefaultAzureCredential());

    services.AddSingleton(openAIClient);

})
.ConfigureFunctionsWebApplication()
.Build();

host.Run();