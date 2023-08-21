namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Libplanet.Blockchain;
using Savor22b.States;

public class ApplianceSpaceStateType : ObjectGraphType<ApplianceSpaceState>
{
    public ApplianceSpaceStateType(BlockChain blockChain)
    {
        Field<NonNullGraphType<IntGraphType>>(
            name: "spaceNumber",
            description: "The ID of the space.",
            resolve: context => context.Source.SpaceNumber
        );
        Field<KitchenEquipmentStateDetailType>(
            name: "installedKitchenEquipment",
            description: "Installed kitchen equipment.",
            resolve: context => GetKitchenEquipmentState(context, blockChain.Count)
        );
    }

    private KitchenEquipmentStateDetail? GetKitchenEquipmentState(
        GraphQL.IResolveFieldContext<ApplianceSpaceState> context,
        long blockIndex
    )
    {
        RootState rootState = (RootState)context.Parent!.Parent!.Parent!.Parent!.Source!;

        if (context.Source.InstalledKitchenEquipmentStateId is not null)
        {
            var kitchenEquipmentState = rootState.InventoryState.GetKitchenEquipmentState(
                context.Source.InstalledKitchenEquipmentStateId!.Value
            );

            if (kitchenEquipmentState is null)
            {
                throw new Exception("Not Found kitchen equipment state id");
            }

            return new KitchenEquipmentStateDetail(
                kitchenEquipmentState,
                rootState.InventoryState,
                blockIndex
            );
        }

        return null;
    }
}
