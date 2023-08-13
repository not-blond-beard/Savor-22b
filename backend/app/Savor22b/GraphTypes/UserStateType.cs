namespace Savor22b.GraphTypes;

using GraphQL.Types;
using Savor22b.States;

public class UserStateType : ObjectGraphType<RootState>
{
    public UserStateType()
    {
        Field<NonNullGraphType<InventoryStateType>>(
            name: "inventoryState",
            description: "The inventory state of the user.",
            resolve: context => context.Source.InventoryState
        );
        Field<VillageStateType>(
            name: "villageState",
            description: "The village state of the user.",
            resolve: context => context.Source.VillageState
        );
    }
}

public class HouseFieldStateType : ObjectGraphType<HouseFieldState>
{
    public HouseFieldStateType()
    {
        Field<NonNullGraphType<GuidGraphType>>(
            name: "InstalledSeedGuid",
            description: "The ID of the installed seed.",
            resolve: context => context.Source.InstalledSeedGuid
        );
        Field<NonNullGraphType<IntGraphType>>(
            name: "SeedID",
            description: "The ID of the seed.",
            resolve: context => context.Source.SeedID
        );
        Field<NonNullGraphType<LongGraphType>>(
            name: "InstalledBlock",
            description: "The block number when the seed was planted.",
            resolve: context => context.Source.InstalledBlock
        );
        Field<NonNullGraphType<IntGraphType>>(
            name: "TotalBlock",
            description: "The total block number of the seed.",
            resolve: context => context.Source.TotalBlock
        );
        Field<LongGraphType>(
            name: "LastWeedBlock",
            description: "The block number when the field was last weeded.",
            resolve: context => context.Source.LastWeedBlock
        );
        Field<NonNullGraphType<IntGraphType>>(
            name: "WeedRemovalCount",
            description: "The number of times the field was weeded.",
            resolve: context => context.Source.WeedRemovalCount
        );
    }
}

public class VillageStateType: ObjectGraphType<VillageState>
{
    public VillageStateType()
    {
        Field<NonNullGraphType<ListGraphType<HouseFieldStateType>>>(
            name: "houseFieldStates",
            description: "The list of house field states in the village.",
            resolve: context => context.Source.HouseFieldStates.ToList()
        );
        Field<NonNullGraphType<HouseStateType>>(
            name: "houseState",
            description: "The house state in the village.",
            resolve: context => context.Source.HouseState
        );
    }
}

public class HouseInnerStateType: ObjectGraphType<HouseInnerState>
{
    public HouseInnerStateType()
    {
        Field<GuidGraphType>(
            name: "FirstBurnerEquipmentID",
            description: "The ID of the first burner equipment.",
            resolve: context => context.Source.FirstBurnerEquipmentID
        );
        Field<GuidGraphType>(
            name: "SecondBurnerEquipmentID",
            description: "The ID of the second burner equipment.",
            resolve: context => context.Source.SecondBurnerEquipmentID
        );
        Field<GuidGraphType>(
            name: "ThirdBurnerEquipmentID",
            description: "The ID of the third burner equipment.",
            resolve: context => context.Source.ThirdBurnerEquipmentID
        );
    }
}

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
        Field<NonNullGraphType<HouseInnerStateType>>(
            name: "innerState",
            description: "The inner state of the house.",
            resolve: context => context.Source.InnerState
        );
    }
}

public class InventoryStateType : ObjectGraphType<InventoryState>
{
    public InventoryStateType()
    {
        Field<ListGraphType<SeedStateType>>(
            name: "seedStateList",
            description: "The list of seed states in the inventory.",
            resolve: context => context.Source.SeedStateList.ToList()
        );

        Field<ListGraphType<RefrigeratorStateType>>(
            name: "refrigeratorStateList",
            description: "The list of refrigerator states in the inventory.",
            resolve: context => context.Source.RefrigeratorStateList.ToList()
        );
    }
}

public class RefrigeratorStateType : ObjectGraphType<RefrigeratorState>
{
    public RefrigeratorStateType()
    {
        Field<GuidGraphType>(
            name: "stateId",
            description: "The ID of the refrigerator state.",
            resolve: context => context.Source.StateID
        );

        Field<IntGraphType>(
            name: "ingredientId",
            description: "The ID of the seed.",
            resolve: context => context.Source.IngredientID
        );

        Field<IntGraphType>(
            name: "foodID",
            description: "The Id of the food.",
            resolve: context => context.Source.FoodID
        );

        Field<StringGraphType>(
            name: "grade",
            description: "The grade of the seed.",
            resolve: context => context.Source.Grade
        );

        Field<IntGraphType>(
            name: "hp",
            description: "The HP of the seed.",
            resolve: context => context.Source.HP
        );

        Field<IntGraphType>(
            name: "attack",
            description: "The attack of the seed.",
            resolve: context => context.Source.ATK
        );

        Field<IntGraphType>(
            name: "defense",
            description: "The defense of the seed.",
            resolve: context => context.Source.DEF
        );

        Field<IntGraphType>(
            name: "speed",
            description: "The speed of the seed.",
            resolve: context => context.Source.SPD
        );
    }
}

public class SeedStateType : ObjectGraphType<SeedState>
{
    public SeedStateType()
    {
        Field<GuidGraphType>(
            name: "stateId",
            description: "The ID of the seed state.",
            resolve: context => context.Source.StateID
        );

        Field<IntGraphType>(
            name: "seedId",
            description: "The ID of the seed.",
            resolve: context => context.Source.SeedID
        );
    }
}
