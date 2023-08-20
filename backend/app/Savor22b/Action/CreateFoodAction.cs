namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Constants;
using Savor22b.Action.Exceptions;
using Savor22b.Action.Util;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;

[ActionType(nameof(CreateFoodAction))]
public class CreateFoodAction : SVRAction
{
    public int RecipeID;
    public Guid FoodStateID;
    public List<Guid> RefrigeratorStateIdsToUse;
    public List<Guid> KitchenEquipmentStateIdsToUse;
    public List<int> ApplianceSpaceNumbersToUse;

    public CreateFoodAction() { }

    public CreateFoodAction(
        int recipeID,
        Guid foodStateID,
        List<Guid> refrigeratorStateIdsToUse,
        List<Guid> kitchenEquipmentStateIdsToUse,
        List<int> applianceSpaceNumbersToUse
    )
    {
        RecipeID = recipeID;
        FoodStateID = foodStateID;
        RefrigeratorStateIdsToUse = refrigeratorStateIdsToUse;
        KitchenEquipmentStateIdsToUse = kitchenEquipmentStateIdsToUse;
        ApplianceSpaceNumbersToUse = applianceSpaceNumbersToUse;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(RecipeID)] = RecipeID.Serialize(),
            [nameof(FoodStateID)] = FoodStateID.Serialize(),
            [nameof(RefrigeratorStateIdsToUse)] = new List(
                RefrigeratorStateIdsToUse.Select(e => e.Serialize())
            ),
            [nameof(KitchenEquipmentStateIdsToUse)] = new List(
                KitchenEquipmentStateIdsToUse.Select(e => e.Serialize())
            ),
            [nameof(ApplianceSpaceNumbersToUse)] = new List(
                ApplianceSpaceNumbersToUse.Select(e => e.Serialize())
            ),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        RecipeID = plainValue[nameof(RecipeID)].ToInteger();
        FoodStateID = plainValue[nameof(FoodStateID)].ToGuid();
        RefrigeratorStateIdsToUse = ((List)plainValue[nameof(RefrigeratorStateIdsToUse)])
            .Select(e => e.ToGuid())
            .ToList();
        KitchenEquipmentStateIdsToUse = ((List)plainValue[nameof(KitchenEquipmentStateIdsToUse)])
            .Select(e => e.ToGuid())
            .ToList();
        ApplianceSpaceNumbersToUse = ((List)plainValue[nameof(ApplianceSpaceNumbersToUse)])
            .Select(e => e.ToInteger())
            .ToList();
    }

    private InventoryState CheckAndRemoveEdibles(
        Recipe recipe,
        InventoryState state,
        List<Guid> refrigeratorStateIdsToUse
    )
    {
        ImmutableList<int> requiredIngredientIds = recipe.IngredientIDList;
        ImmutableList<int> requiredFoodIds = recipe.FoodIDList;

        foreach (var stateId in refrigeratorStateIdsToUse)
        {
            var refrigeratorState = state.GetRefrigeratorItem(stateId);

            if (refrigeratorState is null)
            {
                throw new NotFoundDataException($"You don't have `{stateId}` ingredient or food");
            }

            if (refrigeratorState.GetEdibleType() == Edible.FOOD)
            {
                if (!requiredFoodIds.Contains(refrigeratorState.FoodID!.Value))
                {
                    throw new NotHaveRequiredException(
                        $"You should have foods `{requiredFoodIds}`"
                    );
                }

                requiredFoodIds = requiredFoodIds.Remove(refrigeratorState.FoodID!.Value);
            }
            else if (refrigeratorState.GetEdibleType() == Edible.INGREDIENT)
            {
                if (!requiredIngredientIds.Contains(refrigeratorState.IngredientID!.Value))
                {
                    throw new NotHaveRequiredException(
                        $"You should have foods `{requiredIngredientIds}`"
                    );
                }

                requiredIngredientIds = requiredIngredientIds.Remove(
                    refrigeratorState.IngredientID!.Value
                );
            }

            state = state.RemoveRefrigeratorItem(stateId);
        }

        if (requiredFoodIds.Count != 0 || requiredIngredientIds.Count != 0)
        {
            throw new NotHaveRequiredException(
                $"You should have foods `{requiredFoodIds}` and ingredients `{requiredIngredientIds}`"
            );
        }

        return state;
    }

    private InventoryState CheckAndChangeEquipmentsStatus(
        Recipe recipe,
        InventoryState state,
        List<Guid> kitchenEquipmentStateIdsToUse,
        long currentBlockIndex
    )
    {
        ImmutableList<int> kitchenCategoryIds = recipe.RequiredKitchenEquipmentCategoryList;
        ImmutableList<KitchenEquipmentCategory> kitchenCategories = (
            from categoryId in kitchenCategoryIds
            select CsvDataHelper.GetKitchenEquipmentCategoryByID(categoryId)
        ).ToImmutableList();
        ImmutableList<int> requiredKitchenCategoryIds = (
            from category in kitchenCategories
            where category.Category == "sub"
            select category.ID
        ).ToImmutableList();

        foreach (var stateId in kitchenEquipmentStateIdsToUse)
        {
            var kitchenEquipment = state.GetKitchenEquipmentState(stateId);
            if (kitchenEquipment == null)
            {
                throw new NotFoundDataException($"You don't have `{stateId}` kitchen equipment");
            }

            if (
                !recipe.RequiredKitchenEquipmentCategoryList.Contains(
                    kitchenEquipment.KitchenEquipmentCategoryID
                )
            )
            {
                throw new NotHaveRequiredException(
                    $"You should have equipment category `{kitchenEquipment.KitchenEquipmentCategoryID}`"
                );
            }

            var statusChangedEquipment = kitchenEquipment.StartCooking(
                currentBlockIndex,
                recipe.RequiredBlock
            );
            state = state.RemoveKitchenEquipmentItem(kitchenEquipment.StateID);
            state = state.AddKitchenEquipmentItem(statusChangedEquipment);

            requiredKitchenCategoryIds = requiredKitchenCategoryIds.Remove(
                kitchenEquipment.KitchenEquipmentCategoryID
            );
        }

        if (requiredKitchenCategoryIds.Count != 0)
        {
            throw new NotHaveRequiredException(
                $"You should have kitchenEquipments `{requiredKitchenCategoryIds}`"
            );
        }

        return state;
    }

    private HouseState CheckAndChangeApplianceSpacesStatus(
        Recipe recipe,
        HouseState houseState,
        InventoryState inventoryState,
        List<int> spaceNumbers,
        long currentBlockIndex
    )
    {
        ImmutableList<int> kitchenCategoryIds = recipe.RequiredKitchenEquipmentCategoryList;
        ImmutableList<KitchenEquipmentCategory> kitchenCategories = (
            from categoryId in kitchenCategoryIds
            select CsvDataHelper.GetKitchenEquipmentCategoryByID(categoryId)
        ).ToImmutableList();
        ImmutableList<int> requiredKitchenCategoryIds = (
            from category in kitchenCategories
            where category.Category == "main"
            select category.ID
        ).ToImmutableList();

        foreach (var spaceNumber in spaceNumbers)
        {
            var space = houseState.KitchenState.GetApplianceSpaceStateByNumber(spaceNumber);

            if (!space.EquipmentIsPresent())
            {
                throw new NotFoundDataException($"{spaceNumber} is not installed anything");
            }

            var kitchenEquipment = inventoryState.GetKitchenEquipmentState(
                space.InstalledKitchenEquipmentStateId!.Value
            );

            if (
                !recipe.RequiredKitchenEquipmentCategoryList.Contains(
                    kitchenEquipment.KitchenEquipmentCategoryID
                )
            )
            {
                throw new NotHaveRequiredException(
                    $"You should have equipment category `{kitchenEquipment.KitchenEquipmentCategoryID}`"
                );
            }

            space.StartCooking(currentBlockIndex, recipe.RequiredBlock);

            requiredKitchenCategoryIds = requiredKitchenCategoryIds.Remove(
                kitchenEquipment.KitchenEquipmentCategoryID
            );
        }

        if (requiredKitchenCategoryIds.Count != 0)
        {
            throw new NotHaveRequiredException(
                $"You should have kitchenEquipments `{requiredKitchenCategoryIds}`"
            );
        }

        return houseState;
    }

    private Food FindFoodInCsv(int foodID)
    {
        var Food = CsvDataHelper.GetFoodById(foodID);

        if (Food is null)
        {
            throw new NotFoundTableDataException($"Invalid {nameof(RecipeID)}: {RecipeID}");
        }

        return Food;
    }

    private Stat FindStatInCsv(int foodID, string grade)
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

    private Recipe FindRecipeInCsv(int recipeID)
    {
        var recipe = CsvDataHelper.GetRecipeById(recipeID);

        if (recipe is null)
        {
            throw new NotFoundTableDataException($"Invalid {nameof(recipeID)}: {recipeID}");
        }

        return recipe;
    }

    private (int HP, int ATK, int DEF, int SPD) GenerateStat(IRandom random, Stat stat)
    {
        var hp = random.Next(stat.MinHP, stat.MaxHP + 1);
        var attack = random.Next(stat.MinAtk, stat.MaxAtk + 1);
        var defense = random.Next(stat.MinDef, stat.MaxDef + 1);
        var speed = random.Next(stat.MinSpd, stat.MaxSpd + 1);

        return (HP: hp, ATK: attack, DEF: defense, SPD: speed);
    }

    private RefrigeratorState GenerateFood(Recipe recipe, IRandom random, long currentBlockIndex)
    {
        var gradeExtractor = new GradeExtractor(random, 0.1);

        var foodCsvData = FindFoodInCsv(recipe.ResultFoodID);

        var grade = GradeExtractor.GetGrade(
            gradeExtractor.ExtractGrade(foodCsvData.MinGrade, foodCsvData.MaxGrade)
        );

        var stat = FindStatInCsv(foodCsvData.ID, grade);

        var generatedStat = GenerateStat(random, stat);

        var food = RefrigeratorState.CreateFood(
            FoodStateID,
            RecipeID,
            grade,
            generatedStat.HP,
            generatedStat.ATK,
            generatedStat.DEF,
            generatedStat.SPD,
            currentBlockIndex + recipe.RequiredBlock
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

        RootState rootState = states.GetState(ctx.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        Validation.EnsureVillageStateExists(rootState);

        InventoryState inventoryState = rootState.InventoryState;
        HouseState houseState = rootState.VillageState!.HouseState;

        var recipe = FindRecipeInCsv(RecipeID);

        inventoryState = CheckAndChangeEquipmentsStatus(
            recipe,
            inventoryState,
            KitchenEquipmentStateIdsToUse,
            ctx.BlockIndex
        );
        houseState = CheckAndChangeApplianceSpacesStatus(
            recipe,
            houseState,
            inventoryState,
            ApplianceSpaceNumbersToUse,
            ctx.BlockIndex
        );
        inventoryState = CheckAndRemoveEdibles(recipe, inventoryState, RefrigeratorStateIdsToUse);

        RefrigeratorState food = GenerateFood(recipe, ctx.Random, ctx.BlockIndex);
        inventoryState = inventoryState.AddRefrigeratorItem(food);

        rootState.SetInventoryState(inventoryState);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
