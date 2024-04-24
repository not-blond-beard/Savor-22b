
namespace Savor22b.Tests.Action;

using Libplanet.State;
using Savor22b.Action;
using Savor22b.States;
using Xunit;

public class SellDungeonConquestTests : ActionTests
{
    public static int TestDungeonId = 1;

    public SellDungeonConquestTests() { }

    [Fact]
    public void Execute_Success_Normal()
    {
        var beforeState = CreatePresetStateDelta();

        SellDungeonConquest action = new SellDungeonConquest(TestDungeonId);

        IAccountStateDelta afterState = action.Execute(
            new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = 1,
            }
        );

        var afterGlobalDungeonState = DeriveGlobalDungeonStateDelta(afterState);

        Assert.False(afterGlobalDungeonState.IsDungeonConquestAddress(TestDungeonId, SignerAddress()));
    }

    private IAccountStateDelta CreatePresetStateDelta()
    {
        IAccountStateDelta state = new DummyState();

        GlobalDungeonState globalDungeonState = state.GetState(GlobalDungeonState.StateAddress) is Bencodex.Types.Dictionary globalDungeonStateEncoded
            ? new GlobalDungeonState(globalDungeonStateEncoded)
            : new GlobalDungeonState();

        globalDungeonState = globalDungeonState.SetDungeonConquestAddress(TestDungeonId, SignerAddress());

        return state.SetState(GlobalDungeonState.StateAddress, globalDungeonState.Serialize());
    }
}
