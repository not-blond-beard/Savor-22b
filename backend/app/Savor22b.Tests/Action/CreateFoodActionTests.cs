namespace Savor22b.Tests.Action;

using System;
using Libplanet.State;
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

    private List<RefrigeratorState> generateMaterials(List<int> IngredientIDList, List<int> FoodIDList)
    {
        var RefrigeratorItemList = new List<RefrigeratorState>();
        foreach (var ingredientID in IngredientIDList)
        {
            RefrigeratorItemList.Add(
                RefrigeratorState.CreateIngredient(
                    Guid.NewGuid(),
                    ingredientID,
                    "D",
                    1,
                    1,
                    1,
                    1
                ));
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
                    1
                ));
        }

        return RefrigeratorItemList;
    }

    private (RootState, List<Guid>, List<int>) createPreset(Recipe recipe)
    {
        int spaceNumber = 1;
        List<Guid> kitchenEquipmentsToUse = new List<Guid>();
        List<int> spaceNumbersToUse = new List<int>();

        InventoryState inventoryState = new InventoryState();
        foreach (var item in generateMaterials(recipe.IngredientIDList, recipe.FoodIDList))
        {
            inventoryState = inventoryState.AddRefrigeratorItem(item);
        }

        KitchenState resultKitchenState = new KitchenState();

        foreach (var equipmentCategoryId in recipe.RequiredKitchenEquipmentCategoryList)
        {
            KitchenEquipmentCategory? kitchenEquipmentCategory = CsvDataHelper.GetKitchenEquipmentCategoryByID(equipmentCategoryId);

            if (kitchenEquipmentCategory == null)
            {
                throw new Exception();
            }

            var kitchenEquipments = CsvDataHelper.GetAllKitchenEquipmentByCategoryId(equipmentCategoryId);
            var kitchenEquipmentState = new KitchenEquipmentState(
                    Guid.NewGuid(),
                    kitchenEquipments[0].ID,
                    equipmentCategoryId
                );
            inventoryState = inventoryState.AddKitchenEquipmentItem(kitchenEquipmentState);

            if (kitchenEquipmentCategory.Category == "main")
            {
                resultKitchenState.InstallKitchenEquipment(kitchenEquipmentState, spaceNumber);
                spaceNumbersToUse.Add(spaceNumber);
                spaceNumber++;
            }
            else
            {
                kitchenEquipmentsToUse.Add(kitchenEquipmentState.StateID);
            }
        }

        RootState rootState = new RootState(
            inventoryState,
            new VillageState(new HouseState(1, 1, 1, resultKitchenState)));

        return (rootState, kitchenEquipmentsToUse, spaceNumbersToUse);
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
        var (beforeRootState, kitchenEquipmentStateIdsToUse, spaceNumbersToUse) = createPreset(recipe);
        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var newFoodGuid = Guid.NewGuid();
        var action = new CreateFoodAction(
            recipe.ID,
            newFoodGuid,
            (from stateList in beforeRootState.InventoryState.RefrigeratorStateList
             select stateList.StateID).ToList(),
            kitchenEquipmentStateIdsToUse,
            spaceNumbersToUse);

        var afterState = action.Execute(new DummyActionContext
        {
            PreviousStates = beforeState,
            Signer = SignerAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = blockIndex,
        });

        var rootStateEncoded = afterState.GetState(SignerAddress());
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();
        InventoryState afterInventoryState = rootState.InventoryState;

        Assert.Single(afterInventoryState.RefrigeratorStateList);
        Assert.Equal(recipe.ResultFoodID, afterInventoryState.RefrigeratorStateList[0].FoodID);
        foreach(var kitchenEquipmentStateId in kitchenEquipmentStateIdsToUse)
        {
            Assert.True(afterInventoryState.GetKitchenEquipmentState(kitchenEquipmentStateId)!.IsInUse(blockIndex));
        }
        foreach(var spaceNumber in spaceNumbersToUse)
        {
            switch (spaceNumber)
            {
                case 1:
                    Assert.True(rootState.VillageState!.HouseState.KitchenState.FirstApplianceSpace.IsInUse(blockIndex));
                    break;
                case 2:
                    Assert.True(rootState.VillageState!.HouseState.KitchenState.SecondApplianceSpace.IsInUse(blockIndex));
                    break;
                case 3:
                    Assert.True(rootState.VillageState!.HouseState.KitchenState.ThirdApplianceSpace.IsInUse(blockIndex));
                    break;
                default:
                    throw new Exception("");
            }
        }
    }

    // [Theory]
    // [InlineData(1)]
    // [InlineData(2)]
    // [InlineData(3)]
    // public void Execute_Success_ExistsMoreMaterials(int recipeID)
    // {

    //     Assert.Equal(3, afterInventoryState.RefrigeratorStateList.Count);
    //     Assert.Equal(afterInventoryState.RefrigeratorStateList[0].FoodID, recipe.ResultFoodID);
    //     Assert.Equal(true, afterInventoryState.KitchenEquipmentStateList[0].IsInUse);
    // }

    // [Fact]
    // public void Execute_Failure_AlreadyUsingEquipment()
    // {
    //     Assert.Throws<AlreadyUsingEquipmentException>(() =>
    //     {
    //         action.Execute(new DummyActionContext
    //         {
    //             PreviousStates = beforeState,
    //             Signer = SignerAddress(),
    //             Random = random,
    //             Rehearsal = false,
    //             BlockIndex = 1,
    //         });
    //     });
    // }

    // [Fact]
    // public void Execute_Failure_NotHaveRequiredEquipment()
    // {
    //     Assert.Throws<NotHaveRequiredEquipmentException>(() =>
    //     {
    //         action.Execute(new DummyActionContext
    //         {
    //             PreviousStates = beforeState,
    //             Signer = SignerAddress(),
    //             Random = random,
    //             Rehearsal = false,
    //             BlockIndex = 1,
    //         });
    //     });
    // }

    // [Fact]
    // public void Execute_Failure_NotEnoughRawMaterials()
    // {
    //     Assert.Throws<NotEnoughRawMaterialsException>(() =>
    //     {
    //         action.Execute(new DummyActionContext
    //         {
    //             PreviousStates = beforeState,
    //             Signer = SignerAddress(),
    //             Random = random,
    //             Rehearsal = false,
    //             BlockIndex = 1,
    //         });
    //     });
    // }
}
