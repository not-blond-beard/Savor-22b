using GraphQL.Types;
using Savor22b.States;

public class UserStateType : ObjectGraphType<RootState>
{
    public UserStateType()
    {
        Field<InventoryStateType>(
            name: "inventoryState",
            description: "The inventory state of the user.",
            resolve: context => context.Source.InventoryState
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
            name: "foodId",
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
