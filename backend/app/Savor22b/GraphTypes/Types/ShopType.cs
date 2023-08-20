namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;

public class ShopType : ObjectGraphType<Shop>
{
    public ShopType()
    {
        Field<NonNullGraphType<ListGraphType<ShopKitchenEquipmentType>>>(
            name: "kitchenEquipments",
            description: "This is a list of kitchen equipments that exist within the shop",
            resolve: context => context.Source.KitchenEquipments
        );

        Field<NonNullGraphType<ListGraphType<ItemType>>>(
            name: "items",
            description: "This is a list of default items that exist within the shop",
            resolve: context => context.Source.Items
        );
    }
}
