namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Libplanet.Blockchain;
using Libplanet.Net;
using Libplanet.Store;
using Savor22b.Model;
using Savor22b.States;

public class HouseFieldStateType : ObjectGraphType<HouseFieldState>
{
    public HouseFieldStateType(
        BlockChain blockChain,
        BlockRenderer blockRenderer,
        IStore store,
        Swarm? swarm = null
    )
    {
        Field<NonNullGraphType<GuidGraphType>>(
            name: "InstalledSeedGuid",
            description: "The State ID of the installed seed.",
            resolve: context => context.Source.InstalledSeedGuid
        );
        Field<NonNullGraphType<IntGraphType>>(
            name: "SeedID",
            description: "The ID of the seed.",
            resolve: context => context.Source.SeedID
        );
        Field<NonNullGraphType<LongGraphType>>(
            name: "InstalledBlock",
            description: "The block number when the seed was planted.",
            resolve: context => context.Source.InstalledBlock
        );
        Field<NonNullGraphType<IntGraphType>>(
            name: "TotalBlock",
            description: "The total block number of the seed.",
            resolve: context => context.Source.TotalBlock
        );
        Field<LongGraphType>(
            name: "LastWeedBlock",
            description: "The block number when the field was last weeded.",
            resolve: context => context.Source.LastWeedBlock
        );
        Field<NonNullGraphType<IntGraphType>>(
            name: "WeedRemovalCount",
            description: "The number of times the field was weeded.",
            resolve: context => context.Source.WeedRemovalCount
        );
        Field<NonNullGraphType<StringGraphType>>(
            name: "SeedName",
            description: "The name of the seed.",
            resolve: context =>
            {
                Seed seed = CsvDataHelper.GetSeedById(context.Source.SeedID)!;
                return seed.Name;
            }
        );
        Field<NonNullGraphType<BooleanGraphType>>(
            name: "IsHarvested",
            description: "Whether the seed has been harvested.",
            resolve: context => context.Source.IsHarvestable(blockChain.Count)
        );
        Field<NonNullGraphType<BooleanGraphType>>(
            name: "WeedRemovalAble",
            description: "Whether the field can be weeded.",
            resolve: context => context.Source.WeedRemovalAble(blockChain.Count)
        );
    }
}
