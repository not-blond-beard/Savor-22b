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
    }
}
