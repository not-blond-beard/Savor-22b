
namespace Savor22b.Tests.Action;

using Libplanet.State;
using Libplanet.Assets;
using Savor22b.Action;
using Savor22b.States;
using Xunit;

public class SellDungeonConquestTests : ActionTests
{
    public static int TestDungeonId = 0;

    public SellDungeonConquestTests() { }

    [Fact]
    public void Execute_Success_Normal()
    {
        var (beforeState, conquestStateId) = CreatePresetStateDelta();

        SellDungeonConquest action = new SellDungeonConquest(conquestStateId);

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

        var afterRootState = DeriveRootStateFromAccountStateDelta(afterState);
        UserDungeonState userDungeonState = afterRootState.UserDungeonState;

        Assert.Empty(userDungeonState.DungeonConquestHistories);
        Assert.True(
            FungibleAssetValue.Parse(Currencies.KeyCurrency, "0") < afterState.GetBalance(SignerAddress(), Currencies.KeyCurrency)
        );
    }

    private (IAccountStateDelta, Guid) CreatePresetStateDelta()
    {
        IAccountStateDelta state = new DummyState();

        var rootStateEncoded = state.GetState(SignerAddress());
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : new RootState();

        UserDungeonState userDungeonState = rootState.UserDungeonState;

        var conquestStateId = Guid.NewGuid();
        userDungeonState = userDungeonState.AddDungeonConquestHistory(
            new DungeonConquestHistoryState(conquestStateId, 1, 1, SignerAddress(), 1));

        rootState.SetUserDungeonState(userDungeonState);
        return (state.SetState(SignerAddress(), rootState.Serialize()), conquestStateId);
    }
}
