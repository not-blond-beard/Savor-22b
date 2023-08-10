namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;


[ActionType(nameof(GenerateFoodAction))]
public class GenerateFoodAction : SVRAction
{
    public int RecipeID;
    public Guid FoodStateID;
    public List<Guid> RefrigeratorStateIDs;

    public GenerateFoodAction()
    {
    }

    public GenerateFoodAction(int recipeID, Guid foodStateID, List<Guid> refrigeratorStateIDs)
    {
        RecipeID = recipeID;
        FoodStateID = foodStateID;
        RefrigeratorStateIDs = refrigeratorStateIDs;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(RecipeID)] = RecipeID.Serialize(),
            [nameof(FoodStateID)] = FoodStateID.Serialize(),
            [nameof(RefrigeratorStateIDs)] = new List(RefrigeratorStateIDs.Select(e => e.Serialize())),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        RecipeID = plainValue[nameof(RecipeID)].ToInteger();
        FoodStateID = plainValue[nameof(FoodStateID)].ToGuid();
        RefrigeratorStateIDs = ((List)plainValue[nameof(RefrigeratorStateIDs)]).Select(e => e.ToGuid()).ToList();
    }

    private List<RecipeReference> GetRecipeCSVData()
    {
        CsvParser<RecipeReference> csvParser = new CsvParser<RecipeReference>();

        var csvPath = Paths.GetCSVDataPath("recipe.csv");
        var recipe = csvParser.ParseCsv(csvPath);

        return recipe;
    }

    private List<RecipeStat> GetRecipeStatCSVData()
    {
        CsvParser<RecipeStat> csvParser = new CsvParser<RecipeStat>();

        var csvPath = Paths.GetCSVDataPath("recipe_stat.csv");
        var recipeStat = csvParser.ParseCsv(csvPath);

        return recipeStat;
    }

    private List<Stat> GetStatCSVData()
    {
        CsvParser<Stat> csvParser = new CsvParser<Stat>();

        var csvPath = Paths.GetCSVDataPath("stat.csv");
        var stat = csvParser.ParseCsv(csvPath);

        return stat;
    }

    private RefrigeratorState FindIngredient(InventoryState state, int ingredientID)
    {
        var ingredient = state.RefrigeratorStateList.Find(state => state.IngredientID == ingredientID);

        if (ingredient is null)
        {
            throw new NotEnoughRawMaterialsException($"You don't have `{ingredientID}` ingredient");
        }

        return ingredient;
    }

    private RefrigeratorState FindFood(InventoryState state, int recipeID)
    {
        var food = state.RefrigeratorStateList.Find(state => state.RecipeID == recipeID);

        if (food is null)
        {
            throw new NotEnoughRawMaterialsException($"You don't have `{recipeID}` food");
        }

        return food;
    }

    private List<Guid> FindOwnMaterials(InventoryState state)
    {
        var result = new List<Guid>();
        var recipeList = GetRecipeCSVData();
        var filteredRecipeList = recipeList.FindAll(recipe => recipe.ID == RecipeID);
        foreach (var recipe in filteredRecipeList)
        {
            Guid? stateID = null;
            if (recipe.IngredientID.HasValue)
            {
                stateID = FindIngredient(state, recipe.IngredientID.Value).StateID;
            }
            else if (recipe.ReferencedRecipeID.HasValue)
            {
                stateID = FindFood(state, recipe.ReferencedRecipeID.Value).StateID;
            }

            if (stateID.HasValue)
            {
                result.Add(stateID.Value);
            }
            else
            {
                throw new NotEnoughRawMaterialsException($"You don't have any materials");
            }
        }

        return result;
    }

    private InventoryState RemoveMaterials(InventoryState state, List<Guid> stateIDs)
    {
        InventoryState result = state;
        foreach (var stateID in stateIDs)
        {
            result = result.RemoveRefrigeratorItem(stateID);
        }

        return result;
    }

    private InventoryState CheckAndRemoveForRecipe(InventoryState state)
    {
        InventoryState result = state;

        var materialsForRemoval = FindOwnMaterials(state);
        result = RemoveMaterials(state, materialsForRemoval);

        return result;
    }

    private RecipeStat FindRecipeStat(List<RecipeStat> csvData)
    {
        var recipeStat = csvData.Find(stat => stat.ID == RecipeID);

        if (recipeStat is null)
        {
            throw new NotFoundTableDataException(
                $"Invalid {nameof(RecipeID)}: {RecipeID}");
        }

        return recipeStat;
    }

    private Stat FindStat(List<Stat> csvData, string grade)
    {
        var stat = csvData.Find(stat => stat.RecipeID == RecipeID && stat.Grade == grade);

        if (stat == null)
        {
            throw new NotFoundTableDataException(
                $"Invalid {nameof(grade)}, {nameof(RecipeID)}: {grade}, {RecipeID}");
        }

        return stat;
    }

    private (int HP, int ATK, int DEF, int SPD) GenerateStat(IRandom random, Stat stat)
    {

        var hp = random.Next(stat.MinHP, stat.MaxHP + 1);
        var attack = random.Next(stat.MinAtk, stat.MaxAtk + 1);
        var defense = random.Next(stat.MinDef, stat.MaxDef + 1);
        var speed = random.Next(stat.MinSpd, stat.MaxSpd + 1);

        return (HP: hp, ATK: attack, DEF: defense, SPD: speed);
    }

    private RefrigeratorState GenerateFood(IRandom random)
    {
        var gradeExtractor = new GradeExtractor(random, 0.1);

        var recipeStatCSVData = GetRecipeStatCSVData();
        var recipeStat = FindRecipeStat(recipeStatCSVData);

        var grade = GradeExtractor.GetGrade(
            gradeExtractor.ExtractGrade(recipeStat.MinGrade, recipeStat.MaxGrade));

        var statCSVData = GetStatCSVData();
        var stat = FindStat(statCSVData, grade);

        var generatedStat = GenerateStat(random, stat);

        var food = RefrigeratorState.CreateFood(
            FoodStateID,
            RecipeID,
            grade,
            generatedStat.HP,
            generatedStat.ATK,
            generatedStat.DEF,
            generatedStat.SPD
        );

        return food;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;

        RootState rootState = states.GetState(ctx.Signer) is Bencodex.Types.Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        InventoryState inventoryState = rootState.InventoryState;

        inventoryState = CheckAndRemoveForRecipe(inventoryState);
        RefrigeratorState food = GenerateFood(ctx.Random);
        inventoryState = inventoryState.AddRefrigeratorItem(food);

        rootState.SetInventoryState(inventoryState);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
