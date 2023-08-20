namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class HouseFieldStateType : ObjectGraphType<HouseFieldState>
{
    public HouseFieldStateType()
    {
        Field<NonNullGraphType<GuidGraphType>>(
            name: "InstalledSeedGuid",
            description: "The ID of the installed seed.",
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
    }
}
