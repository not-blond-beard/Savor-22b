namespace Savor22b.Tests.Action;

using System;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;

public class RemoveWeedActionTests : ActionTests
{

    public RemoveWeedActionTests()
    {
    }

    public RootState createRootStatePreset()
    {
        RootState beforeRootState = new RootState(
            new InventoryState(),
            new DungeonState(),
            new VillageState(
                new HouseState(
                    1, 1, 1, new KitchenState()
                )
            )
        );

        return beforeRootState;
    }

    [Fact]
    public void Execute_Success_FirstWeedRemove()
    {
        IAccountStateDelta beforeState = new DummyState();
        RootState beforeRootState = createRootStatePreset();

        HouseFieldState houseFieldState = new(
            Guid.NewGuid(),
            1,
            1,
            10
        );

        beforeRootState.VillageState!.UpdateHouseFieldState(0, houseFieldState);

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        var random = new DummyRandom(1);

        RemoveWeedAction action = new RemoveWeedAction(0);

        IAccountStateDelta state = action.Execute(new DummyActionContext
        {
            PreviousStates = beforeState,
            Signer = SignerAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 5,
        });

        var rootStateEncoded = state.GetState(SignerAddress());

        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Equal(
            5,
            rootState.VillageState!.HouseFieldStates[0]!.LastWeedBlock
        );
        Assert.Equal(
            1,
            rootState.VillageState!.HouseFieldStates[0]!.WeedRemovalCount
        );
        Assert.Equal(
            false,
            rootState.VillageState!.HouseFieldStates[0]!.WeedRemovalAble(6)
        );
    }


    [Fact]
    public void Execute_Success_SecondWeedRemove()
    {
        IAccountStateDelta beforeState = new DummyState();
        RootState beforeRootState = createRootStatePreset();

        HouseFieldState houseFieldState = new(
            Guid.NewGuid(),
            1,
            1,
            10,
            3,
            1
        );

        beforeRootState.VillageState!.UpdateHouseFieldState(0, houseFieldState);

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        var random = new DummyRandom(1);

        RemoveWeedAction action = new RemoveWeedAction(0);

        IAccountStateDelta state = action.Execute(new DummyActionContext
        {
            PreviousStates = beforeState,
            Signer = SignerAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 6,
        });

        var rootStateEncoded = state.GetState(SignerAddress());

        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Equal(
            6,
            rootState.VillageState!.HouseFieldStates[0]!.LastWeedBlock
        );
        Assert.Equal(
            2,
            rootState.VillageState!.HouseFieldStates[0]!.WeedRemovalCount
        );
        Assert.Equal(
            false,
            rootState.VillageState!.HouseFieldStates[0]!.WeedRemovalAble(7)
        );
    }
}
