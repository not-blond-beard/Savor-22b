namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;

public class ShopKitchenEquipmentType : ObjectGraphType<ShopKitchenEquipment>
{
    public ShopKitchenEquipmentType()
    {
        Field<NonNullGraphType<IntGraphType>>(
            name: "id",
            description: "This is the ID of the kitchen equipment",
            resolve: context => context.Source.ID
        );

        Field<NonNullGraphType<IntGraphType>>(
            name: "categoryID",
            description: "This is the ID of the kitchen equipment category",
            resolve: context => context.Source.CategoryID
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "categoryLabel",
            description: "This is the label of the kitchen equipment category",
            resolve: context => context.Source.CategoryLabel
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "categoryType",
            description: "This is the type of the kitchen equipment category",
            resolve: context => context.Source.CategoryType
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "name",
            description: "This is the name of the kitchen equipment",
            resolve: context => context.Source.Name
        );

        Field<NonNullGraphType<FloatGraphType>>(
            name: "blockTimeReductionPercent",
            description: "This is the reduction percentage of block time when using the tool.",
            resolve: context => context.Source.BlockTimeReductionPercent
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "price",
            description: "This is the price of the kitchen equipment",
            resolve: context => context.Source.Price
        );
    }
}
