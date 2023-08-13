namespace Savor22b.Tests.Action;

using Microsoft.Extensions.Configuration;
using Libplanet;
using Libplanet.Crypto;

public class ActionTests
{
    private readonly PrivateKey _signer = new PrivateKey();
    private readonly string csvDataResourcePath;

    public ActionTests()
    {
        string configPath = Environment.GetEnvironmentVariable("SAVOR22B_CONFIG_FILE") ?? "appsettings.local.json";

        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .AddEnvironmentVariables("SAVOR22B_");
        IConfiguration configuration = configurationBuilder.Build();

        csvDataResourcePath = configuration["CsvDataResourcePath"];
        CsvDataHelper.Initialize(csvDataResourcePath);
    }

    public PrivateKey Signer()
    {
        return _signer;
    }

    public Address SignerAddress()
    {
        return _signer.PublicKey.ToAddress();
    }
}
