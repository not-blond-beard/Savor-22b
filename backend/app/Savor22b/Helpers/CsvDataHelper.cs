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
    private static ImmutableList<Food> foodList;
    private static ImmutableList<Recipe> recipeList;
    private static ImmutableList<Stat> statList;
    private static ImmutableList<CookingEquipment> cookingEquipmentList;
    private static ImmutableList<Item> itemList;
    private static ImmutableList<Village> villageList;

    public static void Initialize(string csvDataPath)
    {
        csvDataResourcePath = csvDataPath;
    }

    private static string GetCSVDataPath(string fileName)
    {
        return Path.Combine(csvDataResourcePath, fileName);
    }

    public static string GetRootPath()
    {
        return csvDataResourcePath;
    }

    #region seed

    private static void initSeedCSVData()
    {
        if (seedList == null)
        {
            CsvParser<Seed> csvParser = new CsvParser<Seed>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Seed);
            seedList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<Seed> GetSeedCSVData()
    {
        initSeedCSVData();

        return seedList;
    }

    public static Seed? GetSeedById(int id)
    {
        initSeedCSVData();

        return seedList.Find(seed => seed.Id == id);
    }

    #endregion seed

    #region ingredient

    private static void initIngredientCSVData()
    {
        if (ingredientList == null)
        {
            CsvParser<Ingredient> csvParser = new CsvParser<Ingredient>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Ingredient);
            ingredientList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }

    }

    public static ImmutableList<Ingredient> GetIngredientCSVData()
    {
        initIngredientCSVData();

        return ingredientList;
    }

    public static Ingredient? GetIngredientBySeedId(int seedID)
    {
        initIngredientCSVData();

        return ingredientList.Find(ingredient => ingredient.SeedId == seedID);
    }

    #endregion ingredient

    #region recipe

    private static void initRecipeCSVData()
    {
        if (recipeList == null)
        {
            CsvParser<Recipe> csvParser = new CsvParser<Recipe>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Recipe);
            recipeList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<Recipe> GetRecipeCSVData()
    {
        initRecipeCSVData();

        return recipeList;
    }

    public static Recipe? GetRecipeById(int recipeID)
    {
        initRecipeCSVData();

        return recipeList.Find(recipe => recipe.ID == recipeID);
    }

    #endregion recipe

    #region food

    private static void initFoodCSVData()
    {
        if (foodList == null)
        {
            CsvParser<Food> csvParser = new CsvParser<Food>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Food);
            foodList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<Food> GetFoodCSVData()
    {
        initFoodCSVData();

        return foodList;
    }

    public static Food? GetFoodById(int id)
    {
        initFoodCSVData();

        return foodList.Find(food => food.ID == id);
    }

    #endregion food

    #region stat

    private static void initStatCSVData()
    {
        if (statList == null)
        {
            CsvParser<Stat> csvParser = new CsvParser<Stat>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Stat);
            statList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<Stat> GetStatCSVData()
    {
        initStatCSVData();

        return statList;
    }

    public static Stat? GetStatByIngredientIDAndGrade(int ingredientID, string grade)
    {
        initStatCSVData();

        return statList.Find(stat => stat.IngredientID == ingredientID && stat.Grade == grade);
    }

    public static Stat? GetStatByFoodIDAndGrade(int foodID, string grade)
    {
        initStatCSVData();

        return statList.Find(stat => stat.FoodID == foodID && stat.Grade == grade);
    }

    #endregion stat

    #region cookingEquipment

    private static void initCookingEquipmentCSVData()
    {
        if (cookingEquipmentList == null)
        {
            CsvParser<CookingEquipment> csvParser = new CsvParser<CookingEquipment>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.CookingEquipment);
            cookingEquipmentList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<CookingEquipment> GetCookingEquipmentCSVData()
    {
        initCookingEquipmentCSVData();

        return cookingEquipmentList;
    }

    public static CookingEquipment? GetCookingEquipmentByID(int id)
    {
        initCookingEquipmentCSVData();

        return cookingEquipmentList.Find(equipment => equipment.ID == id);
    }

    #endregion cookingEquipment

    #region item

    private static void initItemCSVData()
    {
        if (itemList == null)
        {
            CsvParser<Item> csvParser = new CsvParser<Item>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Item);
            itemList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<Item> GetItemCSVData()
    {
        initItemCSVData();

        return itemList;
    }

    public static Item? GetItemByID(int id)
    {
        initItemCSVData();

        return itemList.Find(item => item.ID == id);
    }

    #endregion item

    #region village

    private static void initVillageCSVData()
    {
        if (villageList == null)
        {
            CsvParser<Village> csvParser = new CsvParser<Village>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Village);
            villageList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<Village> GetVillageCSVData()
    {
        initVillageCSVData();

        return villageList;
    }

    public static Village? GetVillageByID(int id)
    {
        initVillageCSVData();

        return villageList.Find(village => village.Id == id);
    }

    #endregion village
}
