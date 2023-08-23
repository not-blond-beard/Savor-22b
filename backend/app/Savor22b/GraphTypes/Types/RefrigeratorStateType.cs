namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Libplanet.Blockchain;
using Savor22b.States;

public class RefrigeratorStateType : ObjectGraphType<RefrigeratorState>
{
    public RefrigeratorStateType(BlockChain blockChain)
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
            name: "name",
            description: "The name of the seed.",
            resolve: context => GetRefrigeratorName(context.Source)
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

        Field<BooleanGraphType>(
            name: "isAvailable",
            description: "Check this food is available.",
            resolve: context => context.Source.IsAvailable(blockChain.Count)
        );
    }

    private string GetRefrigeratorName(RefrigeratorState refrigeratorState)
    {
        if (refrigeratorState.IngredientID != null)
        {
            return CsvDataHelper
                .GetIngredientByIngredientId((int)refrigeratorState.IngredientID)!
                .Name;
        }
        else
        {
            return CsvDataHelper.GetFoodById((int)refrigeratorState.FoodID)!.Name;
        }
    }
}
