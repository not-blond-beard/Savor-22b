namespace Savor22b.GraphTypes.Query;

using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Savor22b.Action.Util;
using Savor22b.DataModel;
using Savor22b.GraphTypes.Types;
using Savor22b.Model;

public class CalculateRelocationCostQuery : FieldType
{
    public CalculateRelocationCostQuery()
        : base()
    {
        Name = "calculateRelocationCost";
        Type = typeof(NonNullGraphType<RelocationCostType>);
        Description =
            "Calculating the BBG (Money) and Block Time for Relocation from a Specific Village to Other Villages.";
        Arguments = new QueryArguments(
            new QueryArgument<NonNullGraphType<IntGraphType>>
            {
                Name = "villageId",
                Description = "The ID of the source village for relocation.",
            },
            new QueryArgument<NonNullGraphType<IntGraphType>>
            {
                Name = "relocationVillageId",
                Description = "The ID of the target village where you want to relocate.",
            }
        );
        Resolver = new FuncFieldResolver<RelocationCost>(context =>
        {
            try
            {
                RelocationCost relocationCost = CalculateRelocationCost(
                    context.GetArgument<int>("villageId"),
                    context.GetArgument<int>("relocationVillageId")
                );

                return relocationCost;
            }
            catch (Exception e)
            {
                throw new ExecutionError(e.Message);
            }
        });
    }

    private static RelocationCost CalculateRelocationCost(
        int villageId,
        int targetRelocationVillageId
    )
    {
        Village originVillage = Validation.GetVillage(villageId);
        Village targetVillage = Validation.GetVillage(targetRelocationVillageId);

        RelocationCost relocationCost = VillageUtil.CalculateRelocationCost(
            originVillage.WorldX,
            originVillage.WorldY,
            targetVillage.WorldX,
            targetVillage.WorldY
        );

        return relocationCost;
    }
}
