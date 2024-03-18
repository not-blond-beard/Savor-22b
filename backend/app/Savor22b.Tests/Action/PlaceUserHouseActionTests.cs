namespace Savor22b.Tests.Action;

using System;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;
using Savor22b.Constants;
using Libplanet.Assets;
using Savor22b.Action.Exceptions;

public class PlaceUserHouseActionTests : ActionTests
{
    public PlaceUserHouseActionTests() { }

    [Fact]
    public void Execute_Success_InitialPlaceHouse()
    {
        int villageId = 1;
        int targetX = 0;
        int targetY = 0;

        IAccountStateDelta state = new DummyState();

        var action = new PlaceUserHouseAction(villageId, targetX, targetY);

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

        var globalUserHouseStateEncoded = state.GetState(Addresses.UserHouseDataAddress);

        GlobalUserHouseState globalUserHouseState = globalUserHouseStateEncoded
            is Bencodex.Types.Dictionary stateEncoded
            ? new GlobalUserHouseState(stateEncoded)
            : throw new Exception();

        Assert.Equal(villageId, rootState.VillageState!.HouseState.VillageID);
        Assert.Equal(targetX, rootState.VillageState.HouseState.PositionX);
        Assert.Equal(targetY, rootState.VillageState.HouseState.PositionY);
        Assert.Equal(
            SignerAddress(),
            globalUserHouseState.UserHouse[$"{villageId},{targetX},{targetY}"]
        );
    }

    private IAccountStateDelta createStateForRelocationHouse()
    {
        IAccountStateDelta state = new DummyState();

        GlobalUserHouseState prevGlobalUserHouseState = new GlobalUserHouseState();
        prevGlobalUserHouseState.UserHouse.Add("1,0,0", SignerAddress());

        VillageState villageState = new VillageState(new HouseState(1, 0, 0, new KitchenState()));
        RootState prevRootState = new RootState(
            new InventoryState(),
            new UserDungeonState(),
            villageState
        );

        state = state.MintAsset(
            SignerAddress(),
            FungibleAssetValue.Parse(Currencies.KeyCurrency, "100000")
        );

        state = state.SetState(SignerAddress(), prevRootState.Serialize());
        state = state.SetState(
            Addresses.UserHouseDataAddress,
            prevGlobalUserHouseState.Serialize()
        );

        return state;
    }

    [Fact]
    public void Execute_Success_RelocationHouse()
    {
        int villageId = 2;
        int targetX = 0;
        int targetY = 0;

        IAccountStateDelta state = createStateForRelocationHouse();

        var action = new PlaceUserHouseAction(villageId, targetX, targetY);

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

        var globalUserHouseStateEncoded = state.GetState(Addresses.UserHouseDataAddress);

        GlobalUserHouseState globalUserHouseState = globalUserHouseStateEncoded
            is Bencodex.Types.Dictionary stateEncoded
            ? new GlobalUserHouseState(stateEncoded)
            : throw new Exception();

        Assert.Equal(villageId, rootState.VillageState!.HouseState.VillageID);
        Assert.Equal(targetX, rootState.VillageState.HouseState.PositionX);
        Assert.Equal(targetY, rootState.VillageState.HouseState.PositionY);
        Assert.Equal(
            SignerAddress(),
            globalUserHouseState.UserHouse[$"{villageId},{targetX},{targetY}"]
        );

        Assert.False(globalUserHouseState.CheckPlacedHouse("1,0,0"));
        Assert.Equal(villageId, rootState.RelocationState!.TargetVillageID);

        Assert.True(rootState.RelocationState.IsRelocationInProgress(90));
        Assert.False(rootState.RelocationState.IsRelocationInProgress(91));
        Assert.Equal(1, rootState.RelocationState.StartedBlock);
        Assert.NotEqual(0, rootState.RelocationState.DurationBlock);
    }

    private IAccountStateDelta createStateForInProgressReplaceHouse()
    {
        IAccountStateDelta state = new DummyState();

        GlobalUserHouseState prevGlobalUserHouseState = new GlobalUserHouseState();
        prevGlobalUserHouseState.UserHouse.Add("1,0,0", SignerAddress());

        VillageState villageState = new VillageState(new HouseState(1, 0, 0, new KitchenState()));
        RootState prevRootState = new RootState(
            new InventoryState(),
            new UserDungeonState(),
            villageState,
            new RelocationState(1, 90, 1, 0, 0)
        );

        state = state.MintAsset(
            SignerAddress(),
            FungibleAssetValue.Parse(Currencies.KeyCurrency, "100000")
        );

        state = state.SetState(SignerAddress(), prevRootState.Serialize());
        state = state.SetState(
            Addresses.UserHouseDataAddress,
            prevGlobalUserHouseState.Serialize()
        );

        return state;
    }

    [Fact]
    public void Execute_Failure_InProgressRelocation()
    {
        int villageId = 2;
        int targetX = 0;
        int targetY = 0;

        IAccountStateDelta state = createStateForInProgressReplaceHouse();

        var action = new PlaceUserHouseAction(villageId, targetX, targetY);

        Assert.Throws<RelocationInProgressException>(() =>
        {
            action.Execute(
                new DummyActionContext
                {
                    PreviousStates = state,
                    Signer = SignerAddress(),
                    Random = random,
                    Rehearsal = false,
                    BlockIndex = 50,
                }
            );
        });
    }
}
