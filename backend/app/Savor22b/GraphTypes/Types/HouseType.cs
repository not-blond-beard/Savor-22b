namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.Model;

public class HouseType : ObjectGraphType<House>
{
    public HouseType()
    {
        Field<NonNullGraphType<IntGraphType>>(
            "x",
            description: "The x coordinate of the house.",
            resolve: context => context.Source.X
        );

        Field<NonNullGraphType<IntGraphType>>(
            "y",
            description: "The y coordinate of the house.",
            resolve: context => context.Source.Y
        );

        Field<NonNullGraphType<StringGraphType>>(
            "owner",
            description: "The owner of the house.",
            resolve: context => context.Source.Owner.ToString()
        );
    }
}
