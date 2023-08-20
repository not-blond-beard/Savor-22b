namespace Savor22b.GraphTypes;

using GraphQL.Types;
using Savor22b.Model;

public class VillageType : ObjectGraphType<Village>
{
    public VillageType()
    {
        Field<NonNullGraphType<IntGraphType>>(
            "id",
            description: "The id of the village.",
            resolve: context => context.Source.Id
        );

        Field<NonNullGraphType<StringGraphType>>(
            "name",
            description: "The name of the village.",
            resolve: context => context.Source.Name
        );

        Field<NonNullGraphType<IntGraphType>>(
            "width",
            description: "The width of the village.",
            resolve: context => context.Source.Width
        );

        Field<NonNullGraphType<IntGraphType>>(
            "height",
            description: "The height of the village.",
            resolve: context => context.Source.Height
        );

        Field<NonNullGraphType<IntGraphType>>(
            "worldX",
            description: "The world X position of the village.",
            resolve: context => context.Source.WorldX
        );

        Field<NonNullGraphType<IntGraphType>>(
            "worldY",
            description: "The world Y position of the village.",
            resolve: context => context.Source.WorldY
        );
    }
}
