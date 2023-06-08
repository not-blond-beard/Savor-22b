namespace Savor22b.GraphTypes;

using System.Diagnostics.CodeAnalysis;
using GraphQL;
using GraphQL.Types;
using Libplanet;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Net;
using Savor22b.Helpers;
using Savor22b.Model;

public class Query : ObjectGraphType
{
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "GraphQL docs require long lines of text.")]
    public Query(
        BlockChain blockChain,
        Swarm? swarm = null)
    {
        Field<StringGraphType>(
            "asset",
            description: "The specified address's balance in MNT.",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "address",
                    Description = "The account holder's 40-hex address",
                }
            ),
            resolve: context =>
            {
                var accountAddress = new Address(context.GetArgument<string>("address"));
                FungibleAssetValue asset = blockChain.GetBalance(
                    accountAddress,
                    Currencies.KeyCurrency
                );

                return asset.ToString();
            }
        );

        // TODO: Move to Libplanet.Explorer or Node API.
        Field<StringGraphType>(
            "peerString",
            resolve: context =>
                swarm is null
                ? throw new InvalidOperationException("Network settings is not set.")
                : swarm.AsPeer.PeerString);

        Field<ListGraphType<RecipeGraphType.RecipeType>>(
            "recipe",
            resolve: context =>
                {
                    CsvParser<RecipeReference> recipeCsvParser = new CsvParser<RecipeReference>();
                    var recipeList = recipeCsvParser.ParseCsv(Paths.GetCSVDataPath("recipe.csv"));

                    CsvParser<RecipeStat> recipeStatCsvParser = new CsvParser<RecipeStat>();
                    var recipeStatList = recipeStatCsvParser.ParseCsv(Paths.GetCSVDataPath("recipe_stat.csv"));

                    var recipes = GetRecipeList(recipeList, recipeStatList);

                    return recipes;
                }
        );
    }

    private List<Recipe> GetRecipeList(List<RecipeReference> recipeList, List<RecipeStat> recipeStatList)
    {
        var recipeStatDictionary = CreateRecipeStatDictionary(recipeStatList);

        var recipes = recipeList.GroupBy(recipe => recipe.ID)
            .Select(group => CreateRecipe(group.Key, group.ToList(), recipeStatDictionary))
            .ToList();

        return recipes;
    }

    private Dictionary<int, RecipeStat> CreateRecipeStatDictionary(List<RecipeStat> recipeStatList)
    {
        return recipeStatList.ToDictionary(rs => rs.ID);
    }

    private Recipe CreateRecipe(int recipeId, List<RecipeReference> recipeReferences, Dictionary<int, RecipeStat> recipeStatDictionary)
    {
        var ingredients = recipeReferences.Select(recipeReference =>
        {
            var id = recipeReference.IngredientID ?? recipeReference.ReferencedRecipeID!.Value;
            var name = recipeReference.IngredientID != null
                ? recipeReference.IngredientName!
                : recipeReference.ReferencedRecipeName!;
            var type = recipeReference.IngredientID != null ? "ingredient" : "food";
            return new RecipeIngredient(id, name, type);
        }).ToList();

        var recipeStat = recipeStatDictionary[recipeId];

        return new Recipe(
            recipeId,
            recipeStat.Name,
            ingredients,
            recipeStat.MinGrade,
            recipeStat.MaxGrade
        );
    }
}
