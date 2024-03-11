namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Libplanet.Blockchain;
using Libplanet.Net;
using Libplanet.Store;
using Savor22b.States;

public class DungeonStateType : ObjectGraphType<DungeonState>
{
    public DungeonStateType(
        BlockChain blockChain,
        BlockRenderer blockRenderer,
        IStore store,
        Swarm? swarm = null
    )
    {
        Field<NonNullGraphType<IntGraphType>>(
            name: "DungeonKeyCount",
            description: "The number of dungeon keys the user has.",
            resolve: context => context.Source.GetDungeonKeyCount(blockChain.Count)
        );
    }
}
