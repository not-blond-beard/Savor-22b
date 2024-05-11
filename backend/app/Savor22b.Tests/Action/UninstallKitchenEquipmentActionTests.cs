namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.Action.Exceptions;
using Savor22b.Model;
using Savor22b.States;
using Xunit;

public class UninstallKitchenEquipmentActionTests : ActionTests
{
    public UninstallKitchenEquipmentActionTests() { }

    private readonly Guid EquipmentStateId = new();

    [Fact]
    public void Execute_Success_Normal()
    {
        IAccountStateDelta beforeState = new DummyState();
        RootState beforeRootState = new RootState(
            new InventoryState(
                ImmutableList<SeedState>.Empty,
                ImmutableList<RefrigeratorState>.Empty,
                ImmutableList<KitchenEquipmentState>.Empty.Add(
                    new KitchenEquipmentState(EquipmentStateId, 1, 1)
                ),
                ImmutableList<ItemState>.Empty
            ),
            new UserDungeonState(),
            new VillageState(new HouseState(1, 1, 1, new KitchenState()))
        );
        beforeRootState.VillageState!.HouseState.KitchenState.InstallKitchenEquipment(
            new KitchenEquipmentState(EquipmentStateId, 1, 1),
            1
        );

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var action = new UninstallKitchenEquipmentAction(
            1
        );

        var afterState = action.Execute(
            new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = 1,
            }
        );

        var afterRootStateEncoded = afterState.GetState(SignerAddress());
        RootState afterRootState = afterRootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Null(
            afterRootState
                .VillageState!
                .HouseState
                .KitchenState
                .FirstApplianceSpace
                .InstalledKitchenEquipmentStateId
        );
        Assert.Single(afterRootState.InventoryState.KitchenEquipmentStateList);
    }

    [Fact]
    public void Execute_Failure_NotInstalledKitchenEquipment()
    {
        IAccountStateDelta beforeState = new DummyState();

        RootState beforeRootState = new RootState(
            new InventoryState(
                ImmutableList<SeedState>.Empty,
                ImmutableList<RefrigeratorState>.Empty,
                ImmutableList<KitchenEquipmentState>.Empty,
                ImmutableList<ItemState>.Empty
            ),
            new UserDungeonState(),
            new VillageState(new HouseState(1, 1, 1, new KitchenState()))
        );

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());

        var action = new UninstallKitchenEquipmentAction(1);

        Assert.Throws<InvalidValueException>(() =>
        {
            action.Execute(
                new DummyActionContext
                {
                    PreviousStates = beforeState,
                    Signer = SignerAddress(),
                    Random = random,
                    Rehearsal = false,
                    BlockIndex = 1,
                }
            );
        });
    }
}
