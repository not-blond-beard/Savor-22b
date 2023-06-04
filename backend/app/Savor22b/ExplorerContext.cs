namespace Savor22b;

using Libplanet.Blockchain;
using Libplanet.Explorer.Indexing;
using Libplanet.Explorer.Interfaces;
using Libplanet.Net;
using Libplanet.Store;

public class ExplorerContext : IBlockChainContext
{
    public ExplorerContext(
        BlockChain blockChain,
        IStore store,
        Swarm? swarm = null,
        IBlockChainIndex? index = null
    )
    {
        BlockChain = blockChain;
        Store = store;
        Swarm = swarm;
        Index = index;
    }

    public bool Preloaded => Swarm?.Running ?? true;

    public BlockChain BlockChain { get; private set; }

    public IStore Store { get; private set; }

    public Swarm? Swarm { get; private set; }

    public IBlockChainIndex? Index { get; private set; }
}
