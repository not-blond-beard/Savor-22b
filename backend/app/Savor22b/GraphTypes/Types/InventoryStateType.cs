namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class InventoryStateType : ObjectGraphType<InventoryState>
{
    public InventoryStateType()
    {
        Field<NonNullGraphType<ListGraphType<SeedStateType>>>(
            name: "seedStateList",
            description: "The list of seed states in the inventory.",
            resolve: context => context.Source.SeedStateList.ToList()
        );

        Field<NonNullGraphType<ListGraphType<RefrigeratorStateType>>>(
            name: "refrigeratorStateList",
            description: "The list of refrigerator states in the inventory.",
            resolve: context => context.Source.RefrigeratorStateList.ToList()
        );

        Field<NonNullGraphType<ListGraphType<KitchenEquipmentStateDetailType>>>(
            name: "kitchenEquipmentStateList",
            description: "The list of kitchen equipment states in the inventory.",
            resolve: context =>
                context.Source.KitchenEquipmentStateList
                    .Select(
                        kitchenEquipmentState =>
                            new KitchenEquipmentStateDetail(kitchenEquipmentState)
                    )
                    .ToList()
        );

        Field<NonNullGraphType<ListGraphType<ItemStateDetailType>>>(
            name: "itemStateList",
            description: "The list of item states in the inventory.",
            resolve: context =>
                context.Source.ItemStateList
                    .Select(itemState => new ItemStateDetail(itemState))
                    .ToList()
        );
    }
}
