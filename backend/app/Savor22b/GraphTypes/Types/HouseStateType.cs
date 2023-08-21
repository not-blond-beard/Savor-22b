using GraphQL.Types;
using Savor22b;
using Savor22b.GraphTypes.Types;
using Savor22b.Model;
using Savor22b.States;

public class HouseStateType : ObjectGraphType<HouseState>
{
    public HouseStateType()
    {
        Field<NonNullGraphType<IntGraphType>>(
            name: "villageId",
            description: "The ID of the village.",
            resolve: context => context.Source.VillageID
        );
        Field<NonNullGraphType<IntGraphType>>(
            name: "positionX",
            description: "The X position of the house.",
            resolve: context => context.Source.PositionX
        );
        Field<NonNullGraphType<IntGraphType>>(
            name: "positionY",
            description: "The Y position of the house.",
            resolve: context => context.Source.PositionY
        );
        Field<NonNullGraphType<KitchenStateType>>(
            name: "kitchenState",
            description: "The kitchen state of the house.",
            resolve: context => context.Source.KitchenState
        );
        Field<NonNullGraphType<StringGraphType>>(
            name: "villageName",
            description: "The name of the village.",
            resolve: context =>
            {
                Village village = CsvDataHelper.GetVillageByID(context.Source.VillageID)!;
                return village.Name;
            }
        );
    }
}
