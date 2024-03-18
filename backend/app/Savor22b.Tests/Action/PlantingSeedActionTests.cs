namespace Savor22b.Tests.Action;

using System;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;
using Savor22b.Action.Exceptions;

public class PlantingSeedActionTests : ActionTests
{
    public PlantingSeedActionTests() { }

    private InventoryState getInventoryState()
    {
        InventoryState inventoryState = new InventoryState();

        var itemState = new ItemState(Guid.NewGuid(), 1);
        inventoryState = inventoryState.AddItem(itemState);

        return inventoryState;
    }

    private VillageState getVillageState()
    {
        VillageState villageState = new VillageState(new HouseState(1, 1, 1, new KitchenState()));
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

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());
        var beforeSeedGuid = Guid.NewGuid();

        PlantingSeedAction plantingSeedAction = new PlantingSeedAction(
            beforeSeedGuid,
            0,
            beforeRootState.InventoryState.ItemStateList[0].StateID
        );

        var random = new DummyRandom(1);

        IAccountStateDelta state = plantingSeedAction.Execute(
            new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = 1,
            }
        );

        var rootStateEncoded = state.GetState(SignerAddress());
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Equal(
            rootState.VillageState!.HouseFieldStates[0]!.InstalledSeedGuid,
            beforeSeedGuid
        );
        Assert.Equal(rootState.InventoryState.SeedStateList.Count, 1);
        Assert.Equal(rootState.InventoryState.ItemStateList.Count, 0);
    }

    [Fact]
    public void Execute_InvalidVillageState()
    {
        IAccountStateDelta beforeState = new DummyState();

        InventoryState inventoryState = getInventoryState();
        RootState beforeRootState = new RootState(inventoryState, new UserDungeonState());

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());
        var beforeSeedGuid = Guid.NewGuid();

        PlantingSeedAction plantingSeedAction = new PlantingSeedAction(
            beforeSeedGuid,
            1,
            beforeRootState.InventoryState.ItemStateList[0].StateID
        );

        var random = new DummyRandom(1);

        Assert.Throws<InvalidVillageStateException>(() =>
        {
            plantingSeedAction.Execute(
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

    [Fact]
    public void Execute_InvalidFieldIndex()
    {
        IAccountStateDelta beforeState = new DummyState();

        RootState beforeRootState = createRootStatePreset();

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());
        var beforeSeedGuid = Guid.NewGuid();

        PlantingSeedAction plantingSeedAction = new PlantingSeedAction(
            beforeSeedGuid,
            VillageState.HouseFieldCount + 1,
            beforeRootState.InventoryState.ItemStateList[0].StateID
        );

        var random = new DummyRandom(1);

        Assert.Throws<InvalidFieldIndexException>(() =>
        {
            plantingSeedAction.Execute(
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

    [Fact]
    public void Execute_InvalidItemStateId()
    {
        IAccountStateDelta beforeState = new DummyState();

        RootState beforeRootState = createRootStatePreset();

        beforeState = beforeState.SetState(SignerAddress(), beforeRootState.Serialize());
        var beforeSeedGuid = Guid.NewGuid();

        PlantingSeedAction plantingSeedAction = new PlantingSeedAction(
            beforeSeedGuid,
            0,
            Guid.NewGuid()
        );

        var random = new DummyRandom(1);

        Assert.Throws<NotHaveRequiredException>(() =>
        {
            plantingSeedAction.Execute(
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
