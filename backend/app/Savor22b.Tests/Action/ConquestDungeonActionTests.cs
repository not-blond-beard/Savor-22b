namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.Action.Exceptions;
using Savor22b.Constants;
using Savor22b.States;
using Xunit;

public class ConquestDungeonActionTests : ActionTests
{
    public static int TestDungeonId = 0;

    public ConquestDungeonActionTests() { }

    [Fact]
    public void Execute_Success_InitialConquest()
    {
        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState();
        state.SetUserDungeonState(
            state.UserDungeonState.AddDungeonHistory(
                new DungeonHistoryState(1, TestDungeonId, 1, ImmutableList<int>.Empty)
            )
        );
        beforeState = beforeState.SetState(SignerAddress(), state.Serialize());

        ConquestDungeonAction action = new ConquestDungeonAction(TestDungeonId);

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

        Bencodex.Types.IValue? afterRootStateEncoded = afterState.GetState(SignerAddress());
        RootState afterRootState = afterRootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();
        UserDungeonState userDungeonState = afterRootState.UserDungeonState;

        GlobalDungeonState globalDungeonState = afterState.GetState(Addresses.DungeonDataAddress)
            is Bencodex.Types.Dictionary bdict2
            ? new GlobalDungeonState(bdict2)
            : throw new Exception();

        Assert.True(userDungeonState.DungeonConquestHistories.Count == 1);
        Assert.Equal(1, userDungeonState.DungeonHistories[0].DungeonClearStatus);
        Assert.Equal(TestDungeonId, userDungeonState.DungeonConquestHistories[0].DungeonId);
        Assert.Equal(1, userDungeonState.DungeonConquestHistories[0].DungeonConquestStatus);
        Assert.Equal(globalDungeonState.DungeonConquestAddress(TestDungeonId), SignerAddress());
    }

    [Fact]
    public void Execute_Success_Normal()
    {
        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState();
        state.SetUserDungeonState(
            state.UserDungeonState.AddDungeonHistory(
                new DungeonHistoryState(1, TestDungeonId, 1, ImmutableList<int>.Empty)
            )
        );

        var dungeonState = new GlobalDungeonState();
        dungeonState = dungeonState.SetDungeonConquestAddress(TestDungeonId, new Address());

        beforeState = beforeState
            .SetState(SignerAddress(), state.Serialize())
            .SetState(Addresses.DungeonDataAddress, dungeonState.Serialize());

        ConquestDungeonAction action = new ConquestDungeonAction(TestDungeonId);

        IAccountStateDelta afterState = action.Execute(
            new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = 2,
            }
        );

        Bencodex.Types.IValue? afterRootStateEncoded = afterState.GetState(SignerAddress());
        RootState afterRootState = afterRootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();
        UserDungeonState userDungeonState = afterRootState.UserDungeonState;

        Assert.True(userDungeonState.DungeonConquestHistories.Count == 1);
        Assert.Equal(1, userDungeonState.DungeonHistories[0].DungeonClearStatus);
        Assert.Equal(TestDungeonId, userDungeonState.DungeonConquestHistories[0].DungeonId);
    }

    [Fact]
    public void Execute_Fail_NotClear()
    {
        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState();
        state.SetUserDungeonState(
            state.UserDungeonState.AddDungeonHistory(
                new DungeonHistoryState(1, TestDungeonId, 0, ImmutableList<int>.Empty)
            )
        );
        beforeState = beforeState.SetState(SignerAddress(), state.Serialize());

        ConquestDungeonAction action = new ConquestDungeonAction(TestDungeonId);

        Assert.Throws<DungeonNotClearedException>(
            () =>
                action.Execute(
                    new DummyActionContext
                    {
                        PreviousStates = beforeState,
                        Signer = SignerAddress(),
                        Random = random,
                        Rehearsal = false,
                        BlockIndex = 1,
                    }
                )
        );
    }

    [Fact]
    public void Execute_Fail_NotHaveRequiredDungeonKey()
    {
        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState();
        var userDungeonState = state.UserDungeonState;

        userDungeonState = userDungeonState.AddDungeonHistory(
            new DungeonHistoryState(1, TestDungeonId, 1, ImmutableList<int>.Empty)
        );
        userDungeonState = userDungeonState.AddDungeonConquestHistory(
            new DungeonConquestHistoryState(1, TestDungeonId, 0)
        );
        userDungeonState = userDungeonState.AddDungeonConquestHistory(
            new DungeonConquestHistoryState(2, TestDungeonId, 0)
        );
        userDungeonState = userDungeonState.AddDungeonConquestHistory(
            new DungeonConquestHistoryState(3, TestDungeonId, 0)
        );

        state.SetUserDungeonState(userDungeonState);

        beforeState = beforeState.SetState(SignerAddress(), state.Serialize());

        ConquestDungeonAction action = new ConquestDungeonAction(TestDungeonId);

        Assert.Throws<NotHaveRequiredException>(
            () =>
                action.Execute(
                    new DummyActionContext
                    {
                        PreviousStates = beforeState,
                        Signer = SignerAddress(),
                        Random = random,
                        Rehearsal = false,
                        BlockIndex = 5,
                    }
                )
        );
    }
}
