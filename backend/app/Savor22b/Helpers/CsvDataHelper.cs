namespace Savor22b;

using Savor22b.Constants;
using Savor22b.Helpers;
using Savor22b.Model;
using System.Collections.Immutable;

public static class CsvDataHelper
{
    private static string csvDataResourcePath;
    private static ImmutableList<Seed> seedList;
    private static ImmutableList<Ingredient> ingredientList;
    private static ImmutableList<Recipe> recipeList;

    public static void Initialize(string csvDataPath)
    {
        csvDataResourcePath = csvDataPath;
    }

    private static string GetCSVDataPath(string fileName)
    {
        return Path.Combine(csvDataResourcePath, fileName);
    }

    #region seed

    public static ImmutableList<Seed> GetSeedCSVData()
    {
        if (seedList == null)
        {
            CsvParser<Seed> csvParser = new CsvParser<Seed>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Seed);
            seedList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }

        return seedList;
    }

    #endregion seed
    #region ingredient

    public static ImmutableList<Ingredient> GetIngredientCSVData()
    {
        if (ingredientList == null)
        {
            CsvParser<Ingredient> csvParser = new CsvParser<Ingredient>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Ingredient);
            ingredientList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }

        return ingredientList;
    }
    #endregion ingredient
    #region recipe

    public static ImmutableList<Recipe> GetRecipeCSVData()
    {
        if (recipeList == null)
        {
            CsvParser<Recipe> csvParser = new CsvParser<Recipe>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Recipe);
            recipeList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }

        return recipeList;
    }
    #endregion recipe
}
