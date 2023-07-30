namespace Savor22b.Tests.Action;

using System;
using Libplanet.Crypto;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;
using Libplanet;
using Savor22b.Constants;

public class PlaceUserHouseActionTests
{
    private PrivateKey _signer = new PrivateKey();

    public PlaceUserHouseActionTests()
    {
    }

    [Fact]
    public void PlaceUserHouseActionExecute_UpdateUserHouseState()
    {
        IAccountStateDelta state = new DummyState();

        var random = new DummyRandom(1);
        var action = new PlaceUserHouseAction(1, 0, 0);

        state = action.Execute(new DummyActionContext
        {
            PreviousStates = state,
            Signer = _signer.PublicKey.ToAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var rootStateEncoded = state.GetState(_signer.PublicKey.ToAddress());
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Equal(1, rootState.VillageState!.HouseState.VillageID);
        Assert.Equal(0, rootState.VillageState.HouseState.PositionX);
        Assert.Equal(0, rootState.VillageState.HouseState.PositionY);

        var globalUserHouseStateEncoded = state.GetState(Addresses.UserHouseDataAddress);

        GlobalUserHouseState globalUserHouseState = globalUserHouseStateEncoded is Bencodex.Types.Dictionary stateEncoded
            ? new GlobalUserHouseState(stateEncoded)
            : throw new Exception();

        Assert.Equal(1, globalUserHouseState.UserHouse.Count);
        Assert.Equal(_signer.PublicKey.ToAddress(), globalUserHouseState.UserHouse["1,0,0"]);

    }

    [Theory]
    [InlineData(1, 0, 0)]
    [InlineData(1, 1, 1)]
    [InlineData(2, 0, 0)]
    [InlineData(2, 1, 1)]
    public void PlaceUserHouseActionExecute_UpdateUserHouseStateToExistsUserHouseState(
        int villageId,
        int targetX,
        int targetY
    )
    {
        IAccountStateDelta state = new DummyState();
        RootState rootState = new RootState();

        var random = new DummyRandom(1);

        var action = new PlaceUserHouseAction(villageId, targetX, targetY);

        state = action.Execute(new DummyActionContext
        {
            PreviousStates = state,
            Signer = _signer.PublicKey.ToAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 1,
        });

        var rootStateEncoded = state.GetState(_signer.PublicKey.ToAddress());
        rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Equal(villageId, rootState.VillageState!.HouseState.VillageID);
        Assert.Equal(targetX, rootState.VillageState.HouseState.PositionX);
        Assert.Equal(targetY, rootState.VillageState.HouseState.PositionY);

        var globalUserHouseStateEncoded = state.GetState(Addresses.UserHouseDataAddress);

        GlobalUserHouseState globalUserHouseState = globalUserHouseStateEncoded is Bencodex.Types.Dictionary stateEncoded
            ? new GlobalUserHouseState(stateEncoded)
            : throw new Exception();


        Assert.Equal(_signer.PublicKey.ToAddress(), globalUserHouseState.UserHouse[$"{villageId},{targetX},{targetY}"]);
    }
}
