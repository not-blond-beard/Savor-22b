namespace Savor22b.GraphTypes;

using GraphQL.Types;
using Savor22b.DataModel;

public class RelocationCostType : ObjectGraphType<RelocationCost>
{
    public RelocationCostType()
    {
        Field<NonNullGraphType<IntGraphType>>(
            name: "durationBlocks",
            description: "The number of blocks the relocation cost is valid for.",
            resolve: context => context.Source.DurationBlocks
        );
        Field<NonNullGraphType<DecimalGraphType>>(
            name: "price",
            description: "The relocation cost. (BBG)",
            resolve: context => context.Source.Price.MajorUnit
        );
    }
}
