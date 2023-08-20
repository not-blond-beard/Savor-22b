namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.Model;

public class ItemType : ObjectGraphType<Item>
{
    public ItemType()
    {
        Field<NonNullGraphType<IntGraphType>>(
            name: "id",
            description: "The ID of the item.",
            resolve: context => context.Source.ID
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "name",
            description: "The name of the item.",
            resolve: context => context.Source.Name
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "price",
            description: "The price of the item.",
            resolve: context => context.Source.Price
        );
    }
}
