namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class ApplianceSpaceStateType : ObjectGraphType<ApplianceSpaceState>
{
    public ApplianceSpaceStateType()
    {
        Field<NonNullGraphType<IntGraphType>>(
            name: "spaceNumber",
            description: "The ID of the space.",
            resolve: context => context.Source.SpaceNumber
        );
        Field<GuidGraphType>(
            name: "installedKitchenEquipmentStateId",
            description: "Installed kitchen equipment state id.",
            resolve: context => context.Source.InstalledKitchenEquipmentStateId
        );
        Field<LongGraphType>(
            name: "cookingDurationBlock",
            description: "Total cooking duration block.",
            resolve: context => context.Source.CookingDurationBlock
        );
        Field<LongGraphType>(
            name: "cookingStartedBlockIndex",
            description: "Cooking started block index.",
            resolve: context => context.Source.CookingStartedBlockIndex
        );
    }
}
