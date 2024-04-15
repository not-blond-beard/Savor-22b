namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.Action.Exceptions;
using Savor22b.States;
using Xunit;

public class ExplorationDungeonActionTests : ActionTests
{
    public static int TestDungeonId = 0;

    public ExplorationDungeonActionTests() { }

    [Fact]
    public void Execute_Success_Normal()
    {
        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState();
        beforeState = beforeState.SetState(SignerAddress(), state.Serialize());

        ExplorationDungeonAction action = new ExplorationDungeonAction(TestDungeonId);

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

        Assert.True(userDungeonState.DungeonHistories.Count == 1);
        Assert.Equal(TestDungeonId, userDungeonState.DungeonHistories[0].DungeonId);
        Assert.Equal(UserDungeonState.MaxDungeonKey - 1, userDungeonState.GetDungeonKeyCount(2));
        Assert.True(
            CsvDataHelper
                .GetDungeonById(TestDungeonId)!
                .RewardSeedIdList.SequenceEqual(
                    userDungeonState.DungeonHistories[0].DungeonClearRewardSeedIdList
                )
        );
    }

    [Fact]
    public void Execute_Success_AlreadyUsedKey()
    {
        var currentBlockIndex =
            UserDungeonState.DungeonKeyChargeIntervalBlock * UserDungeonState.MaxDungeonKey + 4;
        var dungeonHistories = ImmutableList.Create(
            new DungeonHistoryState(1, TestDungeonId, 0, ImmutableList<int>.Empty),
            new DungeonHistoryState(2, TestDungeonId, 0, ImmutableList<int>.Empty),
            new DungeonHistoryState(3, TestDungeonId, 0, ImmutableList<int>.Empty),
            new DungeonHistoryState(4, TestDungeonId, 0, ImmutableList<int>.Empty),
            new DungeonHistoryState(5, TestDungeonId, 0, ImmutableList<int>.Empty)
        );

        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState(
            new InventoryState(),
            new UserDungeonState(
                dungeonHistories,
                ImmutableList<DungeonConquestHistoryState>.Empty,
                ImmutableList<DungeonConquestPeriodicRewardHistoryState>.Empty
            )
        );
        beforeState = beforeState.SetState(SignerAddress(), state.Serialize());

        ExplorationDungeonAction action = new ExplorationDungeonAction(TestDungeonId);

        IAccountStateDelta afterState = action.Execute(
            new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = currentBlockIndex,
            }
        );

        Bencodex.Types.IValue? afterRootStateEncoded = afterState.GetState(SignerAddress());
        RootState afterRootState = afterRootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();
        UserDungeonState userDungeonState = afterRootState.UserDungeonState;

        Assert.True(userDungeonState.DungeonHistories.Count == 6);
        Assert.Equal(TestDungeonId, userDungeonState.DungeonHistories[5].DungeonId);
        Assert.Equal(
            UserDungeonState.MaxDungeonKey - 2,
            userDungeonState.GetDungeonKeyCount(currentBlockIndex)
        );
        Assert.True(
            CsvDataHelper
                .GetDungeonById(TestDungeonId)!
                .RewardSeedIdList.SequenceEqual(
                    userDungeonState.DungeonHistories[5].DungeonClearRewardSeedIdList
                )
        );
    }

    [Fact]
    public void Execute_Fail_NotHaveRequiredDungeonKey()
    {
        var currentBlockIndex =
            UserDungeonState.DungeonKeyChargeIntervalBlock * UserDungeonState.MaxDungeonKey + 3;
        var dungeonHistories = ImmutableList.Create(
            new DungeonHistoryState(1, TestDungeonId, 0, ImmutableList<int>.Empty),
            new DungeonHistoryState(4, TestDungeonId, 0, ImmutableList<int>.Empty),
            new DungeonHistoryState(5, TestDungeonId, 0, ImmutableList<int>.Empty),
            new DungeonHistoryState(6, TestDungeonId, 0, ImmutableList<int>.Empty),
            new DungeonHistoryState(7, TestDungeonId, 0, ImmutableList<int>.Empty),
            new DungeonHistoryState(8, TestDungeonId, 0, ImmutableList<int>.Empty)
        );

        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState(
            new InventoryState(),
            new UserDungeonState(
                dungeonHistories,
                ImmutableList<DungeonConquestHistoryState>.Empty,
                ImmutableList<DungeonConquestPeriodicRewardHistoryState>.Empty
            )
        );
        beforeState = beforeState.SetState(SignerAddress(), state.Serialize());

        ExplorationDungeonAction action = new ExplorationDungeonAction(TestDungeonId);

        Assert.Throws<NotHaveRequiredException>(
            () =>
                action.Execute(
                    new DummyActionContext
                    {
                        PreviousStates = beforeState,
                        Signer = SignerAddress(),
                        Random = random,
                        Rehearsal = false,
                        BlockIndex = currentBlockIndex,
                    }
                )
        );
    }

    [Fact]
    public void Execute_Fail_InvalidDungeonId()
    {
        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState();
        beforeState = beforeState.SetState(SignerAddress(), state.Serialize());

        ExplorationDungeonAction action = new ExplorationDungeonAction(-1);

        Assert.Throws<InvalidDungeonException>(
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
}
