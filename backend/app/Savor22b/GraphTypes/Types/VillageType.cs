namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;

public class VillageType : ObjectGraphType<VillageDetail>
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
            description: "The world X position of the village. These coordinates are relative to the center of the World (0,0 coordinates), which means that negative values are also included. (For example, if the width of the World is 5, the x coordinate can range from -2 to 2.)",
            resolve: context => context.Source.WorldX
        );

        Field<NonNullGraphType<IntGraphType>>(
            "worldY",
            description: "The world Y position of the village. These coordinates are relative to the center of the World (0,0 coordinates), which means that negative values are also included. (For example, if the height of the World is 5, the x coordinate can range from -2 to 2.)",
            resolve: context => context.Source.WorldY
        );

        Field<NonNullGraphType<ListGraphType<HouseType>>>(
            "houses",
            description: "This is a list of houses that exist within the village",
            resolve: context => context.Source.Houses
        );

        Field<NonNullGraphType<ListGraphType<DungeonStateType>>>(
            "dungeons",
            description: "마을 내에 위치하는 던전 목록입니다. ",
            resolve: context =>
            {
                var dungeons = CsvDataHelper.GetDungeonCSVData().ToArray();
                return dungeons.Where(d => d.VillageId == context.Source.Id).ToArray();
            }
        );
    }
}
