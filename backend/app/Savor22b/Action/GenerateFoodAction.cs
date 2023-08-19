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
using Savor22b.Action.Util;

[ActionType(nameof(GenerateFoodAction))]
public class GenerateFoodAction : SVRAction
{
    public int RecipeID;
    public Guid FoodStateID;
    public List<Guid> RefrigeratorStateIDs;

    public GenerateFoodAction() { }

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
            [nameof(RefrigeratorStateIDs)] = new List(
                RefrigeratorStateIDs.Select(e => e.Serialize())
            ),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        RecipeID = plainValue[nameof(RecipeID)].ToInteger();
        FoodStateID = plainValue[nameof(FoodStateID)].ToGuid();
        RefrigeratorStateIDs = ((List)plainValue[nameof(RefrigeratorStateIDs)])
            .Select(e => e.ToGuid())
            .ToList();
    }

    private RefrigeratorState FindIngredientInState(InventoryState state, int ingredientID)
    {
        var ingredient = state.RefrigeratorStateList.Find(
            state => state.IngredientID == ingredientID
        );

        if (ingredient is null)
        {
            throw new NotEnoughRawMaterialsException($"You don't have `{ingredientID}` ingredient");
        }

        return ingredient;
    }

    private RefrigeratorState FindFoodInState(InventoryState state, int foodID)
    {
        var food = state.RefrigeratorStateList.Find(state => state.FoodID == foodID);

        if (food is null)
        {
            throw new NotEnoughRawMaterialsException($"You don't have `{foodID}` food");
        }

        return food;
    }

    private List<Guid> FindOwnMaterials(Recipe recipe, InventoryState state)
    {
        List<Guid> resultStateIDList = new List<Guid>();

        foreach (var ingredientID in recipe.IngredientIDList)
        {
            var ingredientStateID = FindIngredientInState(state, ingredientID).StateID;
            resultStateIDList.Add(ingredientStateID);
        }

        foreach (var foodID in recipe.FoodIDList)
        {
            var foodStateID = FindFoodInState(state, foodID).StateID;
            resultStateIDList.Add(foodStateID);
        }

        if (resultStateIDList.Count == 0)
        {
            throw new NotEnoughRawMaterialsException($"You don't have any materials");
        }

        return resultStateIDList;
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

    private InventoryState CheckAndRemoveForRecipe(Recipe recipe, InventoryState state)
    {
        var materialsForRemoval = FindOwnMaterials(recipe, state);
        var result = RemoveMaterials(state, materialsForRemoval);

        return result;
    }

    private Food FindFoodInCSV(int foodID)
    {
        var Food = CsvDataHelper.GetFoodById(foodID);

        if (Food is null)
        {
            throw new NotFoundTableDataException($"Invalid {nameof(RecipeID)}: {RecipeID}");
        }

        return Food;
    }

    private Stat FindStatInCSV(int foodID, string grade)
    {
        var stat = CsvDataHelper.GetStatByFoodIDAndGrade(foodID, grade);

        if (stat == null)
        {
            throw new NotFoundTableDataException(
                $"Invalid {nameof(grade)}, {nameof(foodID)}: {grade}, {foodID}"
            );
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

    private RefrigeratorState GenerateFood(Recipe recipe, IRandom random)
    {
        var gradeExtractor = new GradeExtractor(random, 0.1);

        var foodCsvData = FindFoodInCSV(recipe.ResultFoodID);

        var grade = GradeExtractor.GetGrade(
            gradeExtractor.ExtractGrade(foodCsvData.MinGrade, foodCsvData.MaxGrade)
        );

        var stat = FindStatInCSV(foodCsvData.ID, grade);

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

        RootState rootState = states.GetState(ctx.Signer)
            is Bencodex.Types.Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        Validation.EnsureReplaceInProgress(rootState, ctx.BlockIndex);

        InventoryState inventoryState = rootState.InventoryState;

        var recipe = CsvDataHelper.GetRecipeById(RecipeID);

        if (recipe is null)
        {
            throw new NotFoundTableDataException($"Invalid {nameof(RecipeID)}: {RecipeID}");
        }

        inventoryState = CheckAndRemoveForRecipe(recipe, inventoryState);
        RefrigeratorState food = GenerateFood(recipe, ctx.Random);
        inventoryState = inventoryState.AddRefrigeratorItem(food);

        rootState.SetInventoryState(inventoryState);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
