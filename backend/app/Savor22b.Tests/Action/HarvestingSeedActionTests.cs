namespace Savor22b.Tests.Action;

using System;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;

public class HarvestingSeedActionTests : ActionTests
{

    public HarvestingSeedActionTests()
    {
    }

    [Fact]
    public void Execute_Success_Normal()
    {
        IAccountStateDelta beforeState = new DummyState();
        RootState beforeRootState = new RootState(
            new InventoryState(),
            new VillageState(
                new HouseState(
                    1, 1, 1, new HouseInnerState()
                )
            )
        );

        beforeRootState.VillageState!.UpdateHouseFieldState(0, new HouseFieldState(
            Guid.NewGuid(),
            1,
            1,
            5
        ));

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        var random = new DummyRandom(1);
        HarvestingSeedAction action = new HarvestingSeedAction(
            0,
            Guid.NewGuid()
        );

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

        Assert.Equal(null, rootState.VillageState!.HouseFieldStates[0]);
        Assert.Equal(1, rootState.InventoryState.RefrigeratorStateList.Count);
    }


    [Fact]
    public void Execute_Success_AfterRemoveWeed()
    {
        IAccountStateDelta beforeState = new DummyState();
        RootState beforeRootState = new RootState(
            new InventoryState(),
            new VillageState(
                new HouseState(
                    1, 1, 1, new HouseInnerState()
                )
            )
        );

        beforeRootState.VillageState!.UpdateHouseFieldState(0, new HouseFieldState(
            Guid.NewGuid(),
            1,
            1,
            10,
            4,
            1
        ));

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        var random = new DummyRandom(1);
        HarvestingSeedAction action = new HarvestingSeedAction(
            0,
            Guid.NewGuid()
        );

        IAccountStateDelta state = action.Execute(new DummyActionContext
        {
            PreviousStates = beforeState,
            Signer = SignerAddress(),
            Random = random,
            Rehearsal = false,
            BlockIndex = 10,
        });

        var rootStateEncoded = state.GetState(SignerAddress());

        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();

        Assert.Equal(null, rootState.VillageState!.HouseFieldStates[0]);
        Assert.Equal(1, rootState.InventoryState.RefrigeratorStateList.Count);
    }
}
