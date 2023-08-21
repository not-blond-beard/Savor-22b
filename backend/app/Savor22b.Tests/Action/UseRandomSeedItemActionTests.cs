namespace Savor22b.Tests.Action;

using System;
using Libplanet.Crypto;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;
using Libplanet;

public class UseRandomSeedItemActionTests : ActionTests
{
    public UseRandomSeedItemActionTests() { }

    [Fact]
    public void UseRandomSeedItemActionExecute_AddsSeedToSeedStateList()
    {
        var seedStateID = Guid.NewGuid();
        var random = new DummyRandom(1);
        var villageId = 1;

        IAccountStateDelta state = new DummyState();
        RootState beforeRootState = new RootState();
        InventoryState beforeInventoryState = new InventoryState();

        var itemState = new ItemState(Guid.NewGuid(), 1);
        beforeInventoryState = beforeInventoryState.AddItem(itemState);

        beforeRootState.SetInventoryState(beforeInventoryState);
        beforeRootState.SetVillageState(
            new VillageState(new HouseState(villageId, 0, 0, new KitchenState()))
        );

        state = state.SetState(SignerAddress(), beforeRootState.Serialize());

        var action = new UseRandomSeedItemAction(seedStateID, itemState.StateID);

        state = action.Execute(
            new DummyActionContext
            {
                PreviousStates = state,
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
        InventoryState inventoryState = rootState.InventoryState;

        Assert.Equal(inventoryState.SeedStateList.Count, 1);
        Assert.Equal(inventoryState.ItemStateList.Count, 0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    public void UseRandomSeedItemActionExecute_AddsSeedStateToExistsSeedsList(
        int existsSeedsListLength
    )
    {
        int villageId = 1;

        IAccountStateDelta state = new DummyState();
        var itemState = new ItemState(Guid.NewGuid(), 1);
        RootState rootState = new RootState();
        InventoryState beforeInventoryState = new InventoryState();

        for (int i = 0; i < existsSeedsListLength; i++)
        {
            var newSeed = new SeedState(Guid.NewGuid(), 1);
            beforeInventoryState = beforeInventoryState.AddSeed(newSeed);
        }

        beforeInventoryState = beforeInventoryState.AddItem(itemState);
        rootState.SetInventoryState(beforeInventoryState);
        rootState.SetVillageState(
            new VillageState(new HouseState(villageId, 0, 0, new KitchenState()))
        );

        state = state.SetState(SignerAddress(), rootState.Serialize());

        var random = new DummyRandom(1);

        var action = new UseRandomSeedItemAction(Guid.NewGuid(), itemState.StateID);

        state = action.Execute(
            new DummyActionContext
            {
                PreviousStates = state,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = 1,
            }
        );

        var afterRootStateEncoded = state.GetState(SignerAddress());

        RootState afterRootState = afterRootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Equal(existsSeedsListLength + 1, afterRootState.InventoryState.SeedStateList.Count);
        Assert.Equal(afterRootState.InventoryState.ItemStateList.Count, 0);
    }
}
