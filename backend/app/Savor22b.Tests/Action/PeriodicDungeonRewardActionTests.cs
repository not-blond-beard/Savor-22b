namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.Assets;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.Action.Exceptions;
using Savor22b.Constants;
using Savor22b.States;
using Savor22b.States.Trade;
using Xunit;

public class PeriodicDungeonRewardActionTests : ActionTests
{
    public static int TestDungeonId = 0;

    public PeriodicDungeonRewardActionTests() { }

    [Fact]
    public void Execute_Success_Normal()
    {
        long currentBlock = 402;

        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState(
            new InventoryState(),
            new UserDungeonState(
                ImmutableList.Create(
                    new DungeonHistoryState(1, TestDungeonId, 1, ImmutableList<int>.Empty)
                ),
                ImmutableList.Create(new DungeonConquestHistoryState(1, TestDungeonId, 1)),
                ImmutableList.Create(
                    new DungeonConquestPeriodicRewardHistoryState(
                        105,
                        TestDungeonId,
                        ImmutableList.Create(1, 2, 3)
                    ),
                    new DungeonConquestPeriodicRewardHistoryState(
                        205,
                        TestDungeonId,
                        ImmutableList.Create(1, 2, 3)
                    )
                )
            )
        );

        GlobalDungeonState globalDungeonState = new GlobalDungeonState(
            new Dictionary<string, Address> { [TestDungeonId.ToString()] = SignerAddress() }
        );

        beforeState = beforeState
            .SetState(SignerAddress(), state.Serialize())
            .SetState(Addresses.DungeonDataAddress, globalDungeonState.Serialize());

        PeriodicDungeonRewardAction action = new PeriodicDungeonRewardAction(TestDungeonId);

        IAccountStateDelta afterState = action.Execute(
            new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = currentBlock,
            }
        );

        Bencodex.Types.IValue? afterRootStateEncoded = afterState.GetState(SignerAddress());
        RootState afterRootState = afterRootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : throw new Exception();
        UserDungeonState userDungeonState = afterRootState.UserDungeonState;

        Assert.Equal(
            0,
            userDungeonState.PresentDungeonPeriodicRewardCount(TestDungeonId, 1, currentBlock)
        );
        Assert.Equal(4, userDungeonState.DungeonConquestPeriodicRewardHistories.Count);
        Assert.Equal(5 * 2, afterRootState.InventoryState.SeedStateList.Count);
    }

    [Fact]
    public void Execute_Fail_RegisteredTradeInventory()
    {
        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState(
            new InventoryState(),
            new UserDungeonState(
                ImmutableList.Create(new DungeonHistoryState(1, 2, 1, ImmutableList<int>.Empty)),
                ImmutableList.Create(new DungeonConquestHistoryState(1, 2, 1)),
                ImmutableList<DungeonConquestPeriodicRewardHistoryState>.Empty
            )
        );
        TradeInventoryState tradeInventoryState = new TradeInventoryState();
        tradeInventoryState = tradeInventoryState.RegisterGood(
            new DungeonConquestGoodState(SignerAddress(), Guid.NewGuid(), FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"), 2)
        );

        beforeState = beforeState.SetState(TradeInventoryState.StateAddress, tradeInventoryState.Serialize());
        beforeState = beforeState.SetState(SignerAddress(), state.Serialize());

        PeriodicDungeonRewardAction action = new PeriodicDungeonRewardAction(TestDungeonId);

        Assert.Throws<PermissionDeniedException>(
            () =>
                action.Execute(
                    new DummyActionContext
                    {
                        PreviousStates = beforeState,
                        Signer = SignerAddress(),
                        Random = random,
                        Rehearsal = false,
                        BlockIndex = 200,
                    }
                )
        );
    }

    [Fact]
    public void Execute_Fail_NotConquest()
    {
        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState(
            new InventoryState(),
            new UserDungeonState(
                ImmutableList.Create(new DungeonHistoryState(1, 2, 1, ImmutableList<int>.Empty)),
                ImmutableList.Create(new DungeonConquestHistoryState(1, 2, 1)),
                ImmutableList<DungeonConquestPeriodicRewardHistoryState>.Empty
            )
        );
        beforeState = beforeState.SetState(SignerAddress(), state.Serialize());

        PeriodicDungeonRewardAction action = new PeriodicDungeonRewardAction(TestDungeonId);

        Assert.Throws<PermissionDeniedException>(
            () =>
                action.Execute(
                    new DummyActionContext
                    {
                        PreviousStates = beforeState,
                        Signer = SignerAddress(),
                        Random = random,
                        Rehearsal = false,
                        BlockIndex = 200,
                    }
                )
        );
    }

    [Fact]
    public void Execute_Fail_AlreadyReceive()
    {
        long currentBlock = 299;

        IAccountStateDelta beforeState = new DummyState();
        var state = new RootState(
            new InventoryState(),
            new UserDungeonState(
                ImmutableList.Create(
                    new DungeonHistoryState(1, TestDungeonId, 1, ImmutableList<int>.Empty)
                ),
                ImmutableList.Create(new DungeonConquestHistoryState(1, TestDungeonId, 1)),
                ImmutableList.Create(
                    new DungeonConquestPeriodicRewardHistoryState(
                        105,
                        TestDungeonId,
                        ImmutableList.Create(1, 2, 3)
                    ),
                    new DungeonConquestPeriodicRewardHistoryState(
                        205,
                        TestDungeonId,
                        ImmutableList.Create(1, 2, 3)
                    )
                )
            )
        );

        GlobalDungeonState globalDungeonState = new GlobalDungeonState(
            new Dictionary<string, Address> { [TestDungeonId.ToString()] = SignerAddress() }
        );

        beforeState = beforeState
            .SetState(SignerAddress(), state.Serialize())
            .SetState(Addresses.DungeonDataAddress, globalDungeonState.Serialize());

        PeriodicDungeonRewardAction action = new PeriodicDungeonRewardAction(TestDungeonId);

        Assert.Throws<ResourceIsNotReadyException>(
            () =>
                action.Execute(
                    new DummyActionContext
                    {
                        PreviousStates = beforeState,
                        Signer = SignerAddress(),
                        Random = random,
                        Rehearsal = false,
                        BlockIndex = currentBlock,
                    }
                )
        );
    }
}
