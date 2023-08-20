namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;

public class ItemStateDetailType : ObjectGraphType<ItemStateDetail>
{
    public ItemStateDetailType()
    {
        Field<NonNullGraphType<GuidGraphType>>(
            name: "stateID",
            description: "The ID of the item state.",
            resolve: context => context.Source.StateID
        );

        Field<NonNullGraphType<IntGraphType>>(
            name: "itemID",
            description: "The ID of the item.",
            resolve: context => context.Source.ItemID
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "itemName",
            description: "The name of the item.",
            resolve: context => context.Source.ItemName
        );
    }
}
