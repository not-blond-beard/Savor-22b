namespace Savor22b;

using System.Collections.Immutable;
using Savor22b.Constants;
using Savor22b.Helpers;
using Savor22b.Model;

public static class CsvDataHelper
{
    private static string csvDataResourcePath;
    private static ImmutableList<Seed> seedList;
    private static ImmutableList<Ingredient> ingredientList;
    private static ImmutableList<Food> foodList;
    private static ImmutableList<Recipe> recipeList;
    private static ImmutableList<Stat> statList;
    private static ImmutableList<KitchenEquipment> kitchenEquipmentList;
    private static ImmutableList<KitchenEquipmentCategory> kitchenEquipmentCategoryList;
    private static ImmutableList<Item> itemList;
    private static ImmutableList<Village> villageList;
    private static ImmutableList<Level> levelList;
    private static ImmutableList<VillageCharacteristic> villageCharacteristicList;

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

    public static Ingredient? GetIngredientByIngredientId(int ingredientID)
    {
        initIngredientCSVData();

        return ingredientList.Find(ingredient => ingredient.ID == ingredientID);
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

    #region kitchenEquipment

    private static void initKitchenEquipmentCSVData()
    {
        if (kitchenEquipmentList == null)
        {
            CsvParser<KitchenEquipment> csvParser = new CsvParser<KitchenEquipment>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.KitchenEquipment);
            kitchenEquipmentList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<KitchenEquipment> GetKitchenEquipmentCSVData()
    {
        initKitchenEquipmentCSVData();

        return kitchenEquipmentList;
    }

    public static KitchenEquipment? GetKitchenEquipmentByID(int id)
    {
        initKitchenEquipmentCSVData();

        return kitchenEquipmentList.Find(equipment => equipment.ID == id);
    }

    public static ImmutableList<KitchenEquipment> GetAllKitchenEquipmentByCategoryId(int categoryId)
    {
        initKitchenEquipmentCSVData();

        return kitchenEquipmentList.FindAll(
            equipment => equipment.KitchenEquipmentCategoryID == categoryId
        );
    }

    #endregion kitchenEquipment

    #region kitchenEquipmentCategory

    private static void initKitchenEquipmentCategoryCSVData()
    {
        if (kitchenEquipmentCategoryList == null)
        {
            CsvParser<KitchenEquipmentCategory> csvParser =
                new CsvParser<KitchenEquipmentCategory>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.KitchenEquipmentCategory);
            kitchenEquipmentCategoryList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<KitchenEquipmentCategory> GetKitchenEquipmentCategoryCSVData()
    {
        initKitchenEquipmentCategoryCSVData();

        return kitchenEquipmentCategoryList;
    }

    public static KitchenEquipmentCategory? GetKitchenEquipmentCategoryByID(int id)
    {
        initKitchenEquipmentCategoryCSVData();

        return kitchenEquipmentCategoryList.Find(category => category.ID == id);
    }

    #endregion kitchenEquipmentCategory

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

    #region level

    private static void initLevelCSVData()
    {
        if (levelList == null)
        {
            CsvParser<Level> csvParser = new CsvParser<Level>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.Level);
            levelList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<Level> GetLevelCSVData()
    {
        initLevelCSVData();

        return levelList;
    }

    public static Level? GetLevelByID(int id)
    {
        initLevelCSVData();

        return levelList.Find(level => level.Id == id);
    }

    #endregion level

    #region villageCharacteristic

    private static void initVillageCharacteristic()
    {
        if (villageCharacteristicList == null)
        {
            CsvParser<VillageCharacteristic> csvParser = new CsvParser<VillageCharacteristic>();
            var csvPath = GetCSVDataPath(CsvDataFileNames.VillageCharacteristics);
            villageCharacteristicList = csvParser.ParseCsv(csvPath).ToImmutableList();
        }
    }

    public static ImmutableList<VillageCharacteristic> GetVillageCharacteristicCSVData()
    {
        initVillageCharacteristic();

        return villageCharacteristicList;
    }

    public static VillageCharacteristic? GetVillageCharacteristicByVillageId(int villageId)
    {
        initVillageCharacteristic();

        return villageCharacteristicList.Find(
            characteristic => characteristic.VillageId == villageId
        );
    }

    #endregion villageCharacteristic
}
