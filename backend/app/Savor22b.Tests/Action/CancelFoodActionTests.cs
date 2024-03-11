namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.Action.Exceptions;
using Savor22b.States;
using Xunit;

public class CancelFoodActionTests : ActionTests
{
    private (
        KitchenEquipmentState,
        KitchenEquipmentState,
        RefrigeratorState,
        RootState
    ) createPreset(int blockIndex)
    {
        var equipmentStateId1 = Guid.NewGuid();
        var equipmentStateId2 = Guid.NewGuid();

        var testFood = RefrigeratorState.CreateFood(
            Guid.NewGuid(),
            1,
            "A",
            1,
            1,
            1,
            1,
            blockIndex + 5,
            ImmutableList<Guid>.Empty.Add(equipmentStateId1).Add(equipmentStateId2)
        );

        var kitchenEquipmentState1 = new KitchenEquipmentState(
            equipmentStateId1,
            1,
            1,
            testFood.StateID,
            1,
            10
        );
        var kitchenEquipmentState2 = new KitchenEquipmentState(
            equipmentStateId2,
            2,
            2,
            testFood.StateID,
            1,
            10
        );

        InventoryState beforeInventoryState = new InventoryState();
        beforeInventoryState = beforeInventoryState.AddRefrigeratorItem(testFood);
        beforeInventoryState = beforeInventoryState.AddKitchenEquipmentItem(kitchenEquipmentState1);
        beforeInventoryState = beforeInventoryState.AddKitchenEquipmentItem(kitchenEquipmentState2);

        KitchenState beforeKitchenState = new KitchenState();

        RootState beforeRootState = new RootState(
            beforeInventoryState,
            new DungeonState(),
            new VillageState(new HouseState(1, 1, 1, beforeKitchenState))
        );

        return (kitchenEquipmentState1, kitchenEquipmentState2, testFood, beforeRootState);
    }

    [Fact]
    public void Execute_Success_Normal()
    {
        var blockIndex = 1;
        IAccountStateDelta beforeState = new DummyState();

        var (kitchenEquipmentState1, kitchenEquipmentState2, testFood, beforeRootState) =
            createPreset(blockIndex);

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var action = new CancelFoodAction(testFood.StateID);

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

        Assert.Null(afterRootState.InventoryState.GetRefrigeratorItem(testFood.StateID));

        Assert.NotNull(
            afterRootState.InventoryState.GetKitchenEquipmentState(kitchenEquipmentState1.StateID)
        );
        Assert.False(
            afterRootState.InventoryState
                .GetKitchenEquipmentState(kitchenEquipmentState1.StateID)!
                .IsInUse(blockIndex)
        );
        Assert.NotNull(
            afterRootState.InventoryState.GetKitchenEquipmentState(kitchenEquipmentState2.StateID)
        );
        Assert.False(
            afterRootState.InventoryState
                .GetKitchenEquipmentState(kitchenEquipmentState2.StateID)!
                .IsInUse(blockIndex)
        );
    }

    [Fact]
    public void Execute_Failure_NotCooking()
    {
        var blockIndex = 15;

        IAccountStateDelta beforeState = new DummyState();

        var (kitchenEquipmentState1, kitchenEquipmentState2, testFood, beforeRootState) =
            createPreset(blockIndex - 10);

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var action = new CancelFoodAction(testFood.StateID);

        Assert.Throws<NotCookingException>(() =>
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
    public void Execute_Failure_NotFoundFood()
    {
        var blockIndex = 10;

        IAccountStateDelta beforeState = new DummyState();

        var (kitchenEquipmentState1, kitchenEquipmentState2, testFood, beforeRootState) =
            createPreset(blockIndex);

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var action = new CancelFoodAction(Guid.NewGuid());

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
}
