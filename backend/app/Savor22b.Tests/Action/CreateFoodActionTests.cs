namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet.State;
using Savor22b.Util;
using Savor22b.Action;
using Savor22b.Action.Exceptions;
using Savor22b.Model;
using Savor22b.States;
using Xunit;

public class CreateFoodActionTests : ActionTests
{
    private Recipe getRecipeById(int recipeID)
    {
        var recipe = CsvDataHelper.GetRecipeById(recipeID);

        if (recipe is null)
        {
            throw new Exception();
        }

        return recipe;
    }

    private Recipe getRandomRecipeWithEquipmentCategory(string category)
    {
        var recipeId = 1;
        Recipe? recipe = null;

        while (recipe is null)
        {
            var recipeCandidate = CsvDataHelper.GetRecipeById(recipeId);
            recipeId++;
            if (recipeCandidate is null)
            {
                throw new Exception($"{recipeId} does not exist");
            }

            if (recipeCandidate.RequiredKitchenEquipmentCategoryList.Count == 0)
            {
                continue;
            }

            foreach (
                var equipmentCategoryId in recipeCandidate.RequiredKitchenEquipmentCategoryList
            )
            {
                var kitchenEquipmentCategory = CsvDataHelper.GetKitchenEquipmentCategoryByID(
                    equipmentCategoryId
                );
                if (kitchenEquipmentCategory is null)
                {
                    throw new Exception($"{equipmentCategoryId}");
                }

                if (kitchenEquipmentCategory.Category == category)
                {
                    recipe = recipeCandidate;
                }
            }
        }

        return recipe!;
    }

    private KitchenEquipment getRandomNotRequiredKitchenEquipment(Recipe recipe)
    {
        foreach (var kitchenEquipment in CsvDataHelper.GetKitchenEquipmentCSVData())
        {
            if (
                !recipe.RequiredKitchenEquipmentCategoryList.Contains(
                    kitchenEquipment.KitchenEquipmentCategoryID
                )
            )
            {
                return kitchenEquipment;
            }
        }

        throw new Exception("");
    }

    private List<RefrigeratorState> generateMaterials(
        ImmutableList<int> IngredientIDList,
        ImmutableList<int> FoodIDList
    )
    {
        var RefrigeratorItemList = new List<RefrigeratorState>();
        foreach (var ingredientID in IngredientIDList)
        {
            RefrigeratorItemList.Add(
                RefrigeratorState.CreateIngredient(Guid.NewGuid(), ingredientID, "D", 1, 1, 1, 1)
            );
        }
        foreach (var foodID in FoodIDList)
        {
            RefrigeratorItemList.Add(
                RefrigeratorState.CreateFood(
                    Guid.NewGuid(),
                    foodID,
                    "D",
                    1,
                    1,
                    1,
                    1,
                    1,
                    ImmutableList<Guid>.Empty
                )
            );
        }

        return RefrigeratorItemList;
    }

    private (RootState, List<Guid>) createPreset(Recipe recipe, bool hasBlockReduction = false)
    {
        var spaceNumber = 1;
        List<Guid> kitchenEquipmentsToUse = new List<Guid>();

        InventoryState inventoryState = new InventoryState();
        foreach (var item in generateMaterials(recipe.IngredientIDList, recipe.FoodIDList))
        {
            inventoryState = inventoryState.AddRefrigeratorItem(item);
        }

        KitchenState resultKitchenState = new KitchenState();

        foreach (var equipmentCategoryId in recipe.RequiredKitchenEquipmentCategoryList)
        {
            KitchenEquipmentCategory? kitchenEquipmentCategory =
                CsvDataHelper.GetKitchenEquipmentCategoryByID(equipmentCategoryId);

            if (kitchenEquipmentCategory == null)
            {
                throw new Exception();
            }

            var kitchenEquipments = CsvDataHelper.GetAllKitchenEquipmentByCategoryId(
                equipmentCategoryId
            );
            KitchenEquipment targetKitchenEquipment = null;

            if (hasBlockReduction)
            {
                var higherKitchenEquipment = kitchenEquipments.Find(
                    k => k.BlockTimeReductionPercent > 1
                );
                if (higherKitchenEquipment == null)
                {
                    throw new Exception("");
                }

                targetKitchenEquipment = higherKitchenEquipment;
            }
            else
            {
                targetKitchenEquipment = kitchenEquipments[0];
            }

            var kitchenEquipmentState = new KitchenEquipmentState(
                Guid.NewGuid(),
                targetKitchenEquipment.ID,
                equipmentCategoryId
            );
            inventoryState = inventoryState.AddKitchenEquipmentItem(kitchenEquipmentState);
            kitchenEquipmentsToUse.Add(kitchenEquipmentState.StateID);

            if (kitchenEquipmentCategory.Category == "main")
            {
                resultKitchenState.InstallKitchenEquipment(kitchenEquipmentState, spaceNumber);
                spaceNumber++;
            }
        }

        RootState rootState = new RootState(
            inventoryState,
            new DungeonState(),
            new VillageState(new HouseState(1, 1, 1, resultKitchenState))
        );

        return (rootState, kitchenEquipmentsToUse);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void Execute_Success_Normal(int recipeID)
    {
        var recipe = getRecipeById(recipeID);
        var blockIndex = 1;

        IAccountStateDelta beforeState = new DummyState();
        var (beforeRootState, kitchenEquipmentStateIdsToUse) = createPreset(recipe);
        var edibleStateIdsToUse = (
            from stateList in beforeRootState.InventoryState.RefrigeratorStateList
            select stateList.StateID
        ).ToList();

        var sameIdEdibleOrigin = beforeRootState.InventoryState.RefrigeratorStateList[0];
        var sameIdEdible = new RefrigeratorState(
            Guid.NewGuid(),
            sameIdEdibleOrigin.IngredientID,
            sameIdEdibleOrigin.FoodID,
            sameIdEdibleOrigin.Grade,
            sameIdEdibleOrigin.HP,
            sameIdEdibleOrigin.DEF,
            sameIdEdibleOrigin.ATK,
            sameIdEdibleOrigin.SPD,
            sameIdEdibleOrigin.AvailableBlockIndex,
            sameIdEdibleOrigin.UsedKitchenEquipmentStateIds
        );
        beforeRootState.SetInventoryState(
            beforeRootState.InventoryState.AddRefrigeratorItem(sameIdEdible)
        );
        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var newFoodGuid = Guid.NewGuid();
        var action = new CreateFoodAction(
            recipe.ID,
            newFoodGuid,
            edibleStateIdsToUse,
            kitchenEquipmentStateIdsToUse
        );

        var afterState = action.Execute(
            new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = blockIndex,
            }
        );

        var rootStateEncoded = afterState.GetState(SignerAddress());
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();
        InventoryState afterInventoryState = rootState.InventoryState;

        Assert.NotNull(afterInventoryState.GetRefrigeratorItem(newFoodGuid));
        Assert.NotNull(afterInventoryState.GetRefrigeratorItem(sameIdEdible.StateID));
        Assert.Equal(sameIdEdible, afterInventoryState.GetRefrigeratorItem(sameIdEdible.StateID));
        Assert.Equal(
            recipe.ResultFoodID,
            afterInventoryState.GetRefrigeratorItem(newFoodGuid)!.FoodID
        );
        Assert.Equal(
            blockIndex + recipe.RequiredBlock,
            afterInventoryState.GetRefrigeratorItem(newFoodGuid)!.AvailableBlockIndex
        );
        Assert.False(afterInventoryState.GetRefrigeratorItem(newFoodGuid)!.IsAvailable(blockIndex));
        Assert.True(
            afterInventoryState
                .GetRefrigeratorItem(newFoodGuid)!
                .IsAvailable(blockIndex + recipe.RequiredBlock + 1)
        );
        foreach (var kitchenEquipmentStateId in kitchenEquipmentStateIdsToUse)
        {
            Assert.Equal(
                beforeRootState.InventoryState.GetKitchenEquipmentState(kitchenEquipmentStateId),
                afterInventoryState.GetKitchenEquipmentState(kitchenEquipmentStateId)
            );
            Assert.Equal(
                newFoodGuid,
                afterInventoryState
                    .GetKitchenEquipmentState(kitchenEquipmentStateId)!
                    .CookingFoodStateID
            );
            Assert.True(
                afterInventoryState
                    .GetKitchenEquipmentState(kitchenEquipmentStateId)!
                    .IsInUse(blockIndex)
            );
            Assert.False(
                afterInventoryState
                    .GetKitchenEquipmentState(kitchenEquipmentStateId)!
                    .IsInUse(blockIndex + recipe.RequiredBlock)
            );
        }

        Assert.Equal(
            kitchenEquipmentStateIdsToUse.ToImmutableList(),
            afterInventoryState.GetRefrigeratorItem(newFoodGuid)!.UsedKitchenEquipmentStateIds
        );
    }

    [Fact]
    public void Execute_Success_WithBlockReduction()
    {
        var recipe = getRandomRecipeWithEquipmentCategory("main");
        var blockIndex = 1;

        IAccountStateDelta beforeState = new DummyState();
        var (beforeRootState, kitchenEquipmentStateIdsToUse) = createPreset(recipe, true);
        var edibleStateIdsToUse = (
            from stateList in beforeRootState.InventoryState.RefrigeratorStateList
            select stateList.StateID
        ).ToList();

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var newFoodGuid = Guid.NewGuid();
        var action = new CreateFoodAction(
            recipe.ID,
            newFoodGuid,
            edibleStateIdsToUse,
            kitchenEquipmentStateIdsToUse
        );

        var afterState = action.Execute(
            new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = blockIndex,
            }
        );

        var afterRootStateEncoded = afterState.GetState(SignerAddress());
        RootState afterRootState = afterRootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();
        InventoryState afterInventoryState = afterRootState.InventoryState;

        var sumReductionPercent = 0;
        foreach (var kitchenEquipmentState in afterInventoryState.KitchenEquipmentStateList)
        {
            var kitchenEquipment = CsvDataHelper.GetKitchenEquipmentByID(
                kitchenEquipmentState.KitchenEquipmentID
            );
            sumReductionPercent = sumReductionPercent + kitchenEquipment!.BlockTimeReductionPercent;
        }
        var avgReductionPercent =
            sumReductionPercent / afterInventoryState.KitchenEquipmentStateList.Count;
        var expectedDurationBlock = MathUtil.ReduceByPercentage(
            recipe.RequiredBlock,
            avgReductionPercent
        );

        Assert.Equal(
            expectedDurationBlock,
            afterInventoryState.GetRefrigeratorItem(newFoodGuid).AvailableBlockIndex - blockIndex
        );

        foreach (var kitchenEquipmentStateId in kitchenEquipmentStateIdsToUse)
        {
            Assert.NotNull(
                afterRootState.InventoryState.GetKitchenEquipmentState(kitchenEquipmentStateId)
            );
            Assert.True(
                afterRootState.InventoryState
                    .GetKitchenEquipmentState(kitchenEquipmentStateId)!
                    .IsInUse(blockIndex)
            );
            Assert.Equal(
                expectedDurationBlock,
                afterRootState.InventoryState
                    .GetKitchenEquipmentState(kitchenEquipmentStateId)!
                    .CookingDurationBlock
            );
        }
    }

    [Fact]
    public void Execute_Failure_NotFoundKitchenEquipmentState()
    {
        var blockIndex = 1;
        Recipe recipe = getRandomRecipeWithEquipmentCategory("sub");

        IAccountStateDelta beforeState = new DummyState();
        var (beforeRootState, kitchenEquipmentStateIdsToUse) = createPreset(recipe);
        var edibleStateIdsToUse = (
            from stateList in beforeRootState.InventoryState.RefrigeratorStateList
            select stateList.StateID
        ).ToList();
        beforeRootState.SetInventoryState(
            beforeRootState.InventoryState.RemoveKitchenEquipmentItem(
                kitchenEquipmentStateIdsToUse[0]
            )
        );
        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var newFoodGuid = Guid.NewGuid();
        var action = new CreateFoodAction(
            recipe.ID,
            newFoodGuid,
            edibleStateIdsToUse,
            kitchenEquipmentStateIdsToUse
        );

        Assert.Throws<NotFoundDataException>(() =>
        {
            action.Execute(
                new DummyActionContext
                {
                    PreviousStates = beforeState,
                    Signer = SignerAddress(),
                    Random = random,
                    Rehearsal = false,
                    BlockIndex = blockIndex,
                }
            );
        });
    }

    [Fact]
    public void Execute_Failure_NotFoundEdibleState()
    {
        var blockIndex = 1;
        Recipe recipe = getRecipeById(1);

        IAccountStateDelta beforeState = new DummyState();
        var (beforeRootState, kitchenEquipmentStateIdsToUse) = createPreset(recipe);
        var edibleStateIdsToUse = (
            from stateList in beforeRootState.InventoryState.RefrigeratorStateList
            select stateList.StateID
        ).ToList();
        beforeRootState.SetInventoryState(
            beforeRootState.InventoryState.RemoveRefrigeratorItem(
                beforeRootState.InventoryState.RefrigeratorStateList[0].StateID
            )
        );
        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var newFoodGuid = Guid.NewGuid();
        var action = new CreateFoodAction(
            recipe.ID,
            newFoodGuid,
            edibleStateIdsToUse,
            kitchenEquipmentStateIdsToUse
        );

        Assert.Throws<NotFoundDataException>(() =>
        {
            action.Execute(
                new DummyActionContext
                {
                    PreviousStates = beforeState,
                    Signer = SignerAddress(),
                    Random = random,
                    Rehearsal = false,
                    BlockIndex = blockIndex,
                }
            );
        });
    }

    [Fact]
    public void Execute_Failure_NotHaveRequiredKitchenEquipmentState()
    {
        var blockIndex = 1;
        Recipe recipe = getRandomRecipeWithEquipmentCategory("sub");
        KitchenEquipment notRequiredKitchenEquipment = getRandomNotRequiredKitchenEquipment(recipe);

        IAccountStateDelta beforeState = new DummyState();
        var (beforeRootState, kitchenEquipmentStateIdsToUse) = createPreset(recipe);
        var edibleStateIdsToUse = (
            from stateList in beforeRootState.InventoryState.RefrigeratorStateList
            select stateList.StateID
        ).ToList();
        beforeRootState.SetInventoryState(
            beforeRootState.InventoryState.RemoveKitchenEquipmentItem(
                kitchenEquipmentStateIdsToUse[0]
            )
        );
        beforeRootState.SetInventoryState(
            beforeRootState.InventoryState.AddKitchenEquipmentItem(
                new KitchenEquipmentState(
                    kitchenEquipmentStateIdsToUse[0],
                    notRequiredKitchenEquipment.ID,
                    notRequiredKitchenEquipment.KitchenEquipmentCategoryID
                )
            )
        );
        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var newFoodGuid = Guid.NewGuid();
        var action = new CreateFoodAction(
            recipe.ID,
            newFoodGuid,
            edibleStateIdsToUse,
            kitchenEquipmentStateIdsToUse
        );

        Assert.Throws<NotHaveRequiredException>(() =>
        {
            action.Execute(
                new DummyActionContext
                {
                    PreviousStates = beforeState,
                    Signer = SignerAddress(),
                    Random = random,
                    Rehearsal = false,
                    BlockIndex = blockIndex,
                }
            );
        });
    }

    [Fact]
    public void Execute_Failure_NotHaveRequiredEdibleState()
    {
        var blockIndex = 1;
        Recipe recipe = getRecipeById(1);

        IAccountStateDelta beforeState = new DummyState();
        var (beforeRootState, kitchenEquipmentStateIdsToUse) = createPreset(recipe);
        beforeRootState.SetInventoryState(
            beforeRootState.InventoryState.RemoveRefrigeratorItem(
                beforeRootState.InventoryState.RefrigeratorStateList[0].StateID
            )
        );
        beforeRootState.SetInventoryState(
            beforeRootState.InventoryState.AddRefrigeratorItem(
                RefrigeratorState.CreateIngredient(Guid.NewGuid(), -1, "A", 1, 1, 1, 1)
            )
        );
        var edibleStateIdsToUse = (
            from stateList in beforeRootState.InventoryState.RefrigeratorStateList
            select stateList.StateID
        ).ToList();
        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var newFoodGuid = Guid.NewGuid();
        var action = new CreateFoodAction(
            recipe.ID,
            newFoodGuid,
            edibleStateIdsToUse,
            kitchenEquipmentStateIdsToUse
        );

        Assert.Throws<NotHaveRequiredException>(() =>
        {
            action.Execute(
                new DummyActionContext
                {
                    PreviousStates = beforeState,
                    Signer = SignerAddress(),
                    Random = random,
                    Rehearsal = false,
                    BlockIndex = blockIndex,
                }
            );
        });
    }
}
