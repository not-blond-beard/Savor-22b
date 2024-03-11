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
            description: "The ID of the edible.",
            resolve: context => context.Source.IngredientID
        );

        Field<IntGraphType>(
            name: "foodID",
            description: "The Id of the edible.",
            resolve: context => context.Source.FoodID
        );

        Field<StringGraphType>(
            name: "name",
            description: "The name of the edible.",
            resolve: context => GetRefrigeratorName(context.Source)
        );

        Field<StringGraphType>(
            name: "grade",
            description: "The grade of the edible.",
            resolve: context => context.Source.Grade
        );

        Field<IntGraphType>(
            name: "hp",
            description: "The HP of the edible.",
            resolve: context => GetCalculatedStat(context.Source.Level, context.Source.HP)
        );

        Field<IntGraphType>(
            name: "attack",
            description: "The attack of the edible.",
            resolve: context => GetCalculatedStat(context.Source.Level, context.Source.ATK)
        );

        Field<IntGraphType>(
            name: "defense",
            description: "The defense of the edible.",
            resolve: context => GetCalculatedStat(context.Source.Level, context.Source.DEF)
        );

        Field<IntGraphType>(
            name: "speed",
            description: "The speed of the edible.",
            resolve: context => GetCalculatedStat(context.Source.Level, context.Source.SPD)
        );

        Field<BooleanGraphType>(
            name: "isSuperFood",
            description: "Check this food is SuperFood.",
            resolve: context => context.Source.IsSuperFood
        );

        Field<IntGraphType>(
            name: "level",
            description: "The level of the edible",
            resolve: context => context.Source.Level
        );

        Field<BooleanGraphType>(
            name: "isAvailable",
            description: "Check this edible is available.",
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

    private int GetCalculatedStat(int level, int value)
    {
        var levelInfo = CsvDataHelper.GetLevelCSVData();

        double upgradedStat = 0;
        for (int i = 0; i < level; i++)
        {
            upgradedStat += value / 100 * levelInfo[i].Increase;
        }

        return value + (int)upgradedStat;
    }
}
