namespace Libplanet.Headless.Hosting;

using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Net;
using Libplanet.Store;
using Microsoft.Extensions.DependencyInjection;

public static class LibplanetServicesExtensions
{
    public static IServiceCollection AddLibplanet(
        this IServiceCollection services,
        Action<ILibplanetBuilder> configure
    )
    {
        var builder = new LibplanetBuilder();
        configure(builder);
        InstantiatedNodeComponents build = builder.Build();

        services.AddSingleton<IBlockPolicy>(build.BlockChain.Policy);
        services.AddSingleton<IStagePolicy>(build.BlockChain.StagePolicy);
        services.AddSingleton<IStore>(build.Store);
        services.AddSingleton<IStateStore>(build.StateStore);
        services.AddSingleton<BlockChain>(build.BlockChain);

        if (build.ValidatorDriverConfiguration is { } validatorDriverConfiguration)
        {
            services.AddSingleton<ValidatorDriverConfiguration>(validatorDriverConfiguration);
        }

        if (build.ValidatorPrivateKey is { } validatorPrivateKey)
        {
            services.AddSingleton<ValidatorPrivateKey>(new ValidatorPrivateKey(validatorPrivateKey));
        }

        if (build.Swarm is { } swarm && build.BootstrapMode is { } bootstrapMode)
        {
            services.AddSingleton<Swarm>(swarm);
            services.AddSingleton(typeof(SwarmService.BootstrapMode), bootstrapMode);
            services.AddHostedService<SwarmService>();
        }
        else if (build.ValidatorPrivateKey is not null)
        {
            services.AddHostedService<SoloValidationService>();
        }

        return services;
    }
}
