namespace Savor22b.Tests.Action;

using System;
using Libplanet.State;
using Xunit;
using Savor22b.Action;
using Savor22b.States;

public class RemovePlantedSeedActionTests : ActionTests
{

    public RemovePlantedSeedActionTests()
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
            5,
            null
        ));

        beforeState = beforeState.SetState(
            SignerAddress(),
            beforeRootState.Serialize()
        );

        var random = new DummyRandom(1);
        RemovePlantedSeedAction action = new RemovePlantedSeedAction(0);

        IAccountStateDelta state = action.Execute(new DummyActionContext
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

        Assert.Equal(null, rootState.VillageState!.HouseFieldStates[0]);
        Assert.Equal(0, rootState.InventoryState.SeedStateList.Count);
    }
}
