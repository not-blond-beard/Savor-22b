namespace Libplanet.Headless.Hosting;

using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Store;
using Libplanet.Net;



public record InstantiatedNodeComponents(
    IStore Store,
    IStateStore StateStore,
    BlockChain BlockChain,
    Swarm? Swarm,
    SwarmService.BootstrapMode? BootstrapMode,
    PrivateKey? ValidatorPrivateKey,
    ValidatorDriverConfiguration ValidatorDriverConfiguration
);
