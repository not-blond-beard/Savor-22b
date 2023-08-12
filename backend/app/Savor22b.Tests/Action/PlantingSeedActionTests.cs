namespace Savor22b.Tests.Action;

using System;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;
using Savor22b.Action.Exceptions;
using System.Collections.Immutable;

public class PlantingSeedActionTests : ActionTests
{

    public PlantingSeedActionTests()
    {
    }

    private InventoryState getInventoryState()
    {
        InventoryState inventoryState = new InventoryState();
        var newSeed = new SeedState(Guid.NewGuid(), 1);
        inventoryState = inventoryState.AddSeed(newSeed);
        return inventoryState;
    }

    private VillageState getVillageState()
    {
        VillageState villageState = new VillageState(
            new HouseState(
                1,
                1,
                1,
                new HouseInnerState()
            )
        );
        return villageState;
    }

    private RootState createRootStatePreset()
    {
        RootState rootState = new RootState();
        InventoryState inventoryState = getInventoryState();
        VillageState villageState = getVillageState();

        rootState.SetVillageState(villageState);
        rootState.SetInventoryState(inventoryState);

        return rootState;
    }

    [Fact]
    public void Execute_ValidAction()
    {
        IAccountStateDelta beforeState = new DummyState();
        RootState beforeRootState = createRootStatePreset();

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        PlantingSeedAction plantingSeedAction = new PlantingSeedAction(
            beforeRootState.InventoryState.SeedStateList[0].StateID,
            0
        );

        var random = new DummyRandom(1);

        IAccountStateDelta state = plantingSeedAction.Execute(new DummyActionContext
        {
            PreviousStates = beforeState,
            Signer = SignerAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var rootStateEncoded = state.GetState(SignerAddress());
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Equal(rootState.VillageState!.HouseFieldStates[0]!.InstalledSeedGuid, beforeRootState.InventoryState.SeedStateList[0].StateID);
        Assert.Equal(rootState.InventoryState.SeedStateList.Count, 0);
    }

    [Fact]
    public void Execute_InvalidVillageState()
    {
        IAccountStateDelta beforeState = new DummyState();

        InventoryState inventoryState = getInventoryState();
        RootState beforeRootState = new RootState(
            inventoryState
        );

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        PlantingSeedAction plantingSeedAction = new PlantingSeedAction(
            beforeRootState.InventoryState.SeedStateList[0].StateID,
            1
        );

        var random = new DummyRandom(1);

        Assert.Throws<InvalidVillageStateException>(() =>
        {
            plantingSeedAction.Execute(new DummyActionContext
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
    public void Execute_InvalidFieldIndex()
    {
        IAccountStateDelta beforeState = new DummyState();

        RootState beforeRootState = createRootStatePreset();

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        PlantingSeedAction plantingSeedAction = new PlantingSeedAction(
            beforeRootState.InventoryState.SeedStateList[0].StateID,
            VillageState.HouseFieldCount + 1
        );

        var random = new DummyRandom(1);

        Assert.Throws<InvalidFieldIndexException>(() =>
        {
            plantingSeedAction.Execute(new DummyActionContext
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
    public void Execute_InvalidSeedStateId()
    {
        IAccountStateDelta beforeState = new DummyState();

        RootState beforeRootState = createRootStatePreset();

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        PlantingSeedAction plantingSeedAction = new PlantingSeedAction(
            Guid.NewGuid(),
            0
        );

        var random = new DummyRandom(1);

        Assert.Throws<NotFoundDataException>(() =>
        {
            plantingSeedAction.Execute(new DummyActionContext
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
