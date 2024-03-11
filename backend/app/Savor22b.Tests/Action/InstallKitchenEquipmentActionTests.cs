namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.Action.Exceptions;
using Savor22b.Model;
using Savor22b.States;
using Xunit;

public class InstallKitchenEquipmentActionTests : ActionTests
{
    public InstallKitchenEquipmentActionTests()
    {
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Execute_Success_NormalCase(int spaceNumber)
    {
        IAccountStateDelta beforeState = new DummyState();

        RootState beforeRootState = new RootState(
            new InventoryState(
                ImmutableList<SeedState>.Empty,
                ImmutableList<RefrigeratorState>.Empty,
                ImmutableList<KitchenEquipmentState>.Empty.Add(new KitchenEquipmentState(Guid.NewGuid(), 1, 1)),
                ImmutableList<ItemState>.Empty
            ),
            new DungeonState(),
            new VillageState(
                new HouseState(
                    1, 1, 1, new KitchenState()
                )
            )
        );

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        var action = new InstallKitchenEquipmentAction(beforeRootState.InventoryState.KitchenEquipmentStateList[0].StateID, spaceNumber);

        var afterState = action.Execute(new DummyActionContext
        {
            PreviousStates = beforeState,
            Signer = SignerAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var afterRootStateEncoded = afterState.GetState(SignerAddress());
        RootState afterRootState = afterRootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Single(afterRootState.InventoryState.KitchenEquipmentStateList);
        Assert.Equal(
            beforeRootState.InventoryState.KitchenEquipmentStateList[0].StateID,
            afterRootState.InventoryState.KitchenEquipmentStateList[0].StateID);

        if (spaceNumber == 1)
        {
            Assert.Equal(
                beforeRootState.InventoryState.KitchenEquipmentStateList[0].StateID,
                afterRootState.VillageState!.HouseState.KitchenState.FirstApplianceSpace.InstalledKitchenEquipmentStateId);
        }
        else if (spaceNumber == 2)
        {
            Assert.Equal(
                beforeRootState.InventoryState.KitchenEquipmentStateList[0].StateID,
                afterRootState.VillageState!.HouseState.KitchenState.SecondApplianceSpace.InstalledKitchenEquipmentStateId);
        }
        else if (spaceNumber == 3)
        {
            Assert.Equal(
                beforeRootState.InventoryState.KitchenEquipmentStateList[0].StateID,
                afterRootState.VillageState!.HouseState.KitchenState.ThirdApplianceSpace.InstalledKitchenEquipmentStateId);
        }
        else
        {
            throw new Exception("Not handled space number");
        }
    }

    [Fact]
    public void Execute_Failure_NotHaveKitchenEquipment()
    {
        IAccountStateDelta beforeState = new DummyState();

        RootState beforeRootState = new RootState(
            new InventoryState(
                ImmutableList<SeedState>.Empty,
                ImmutableList<RefrigeratorState>.Empty,
                ImmutableList<KitchenEquipmentState>.Empty,
                ImmutableList<ItemState>.Empty
            ),
            new DungeonState(),
            new VillageState(
                new HouseState(
                    1, 1, 1, new KitchenState()
                )
            )
        );

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        var action = new InstallKitchenEquipmentAction(Guid.NewGuid(), 1);

        Assert.Throws<NotHaveRequiredException>(() =>
        {
            action.Execute(new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = 1,
            });
        });
    }

    [Fact]
    public void Execute_Failure_NotPlacedHouse()
    {
        IAccountStateDelta beforeState = new DummyState();

        RootState beforeRootState = new RootState(
            new InventoryState(
                ImmutableList<SeedState>.Empty,
                ImmutableList<RefrigeratorState>.Empty,
                ImmutableList<KitchenEquipmentState>.Empty,
                ImmutableList<ItemState>.Empty
            ),
            new DungeonState(),
            null
        );

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        var action = new InstallKitchenEquipmentAction(Guid.NewGuid(), 1);

        Assert.Throws<InvalidVillageStateException>(() =>
        {
            action.Execute(new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = 1,
            });
        });
    }
}
