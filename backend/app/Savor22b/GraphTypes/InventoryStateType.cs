using GraphQL.Types;
using Savor22b.States;

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
        Field<IntGraphType>(
            name: "id",
            description: "The ID of the refrigerator state.",
            resolve: context => context.Source.Id
        );

        Field<IntGraphType>(
            name: "ingredientId",
            description: "The ID of the seed.",
            resolve: context => context.Source.IngredientId
        );

        Field<StringGraphType>(
            name: "recipeId",
            description: "The Id of the recipe.",
            resolve: context => context.Source.RecipeId
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
        Field<IntGraphType>(
            name: "id",
            description: "The ID of the seed state.",
            resolve: context => context.Source.Id
        );

        Field<IntGraphType>(
            name: "seedId",
            description: "The ID of the seed.",
            resolve: context => context.Source.SeedID
        );
    }
}
