namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.State;
using NetMQ;
using Savor22b.Action;
using Savor22b.Action.Exceptions;
using Savor22b.States;
using Xunit;

public class LevelUpActionTests : ActionTests
{
    private static readonly int MsgItemId = 3;

    [Fact]
    public void UseLevelUpAction_Success()
    {
        var stateDelta = CreatePresetStateDelta();
        var beforeState = DeriveRootStateFromAccountStateDelta(stateDelta);

        var beforeFood = beforeState.InventoryState.RefrigeratorStateList[0];

        var action = new LevelUpAction(beforeFood.StateID);
        stateDelta = action.Execute(
            new DummyActionContext
            {
                PreviousStates = stateDelta,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = 1,
            }
        );

        var afterState = DeriveRootStateFromAccountStateDelta(stateDelta);
        var afterFood = beforeState.InventoryState.RefrigeratorStateList[0];

        Assert.True(beforeState.InventoryState.ItemStateList.Count > afterState.InventoryState.ItemStateList.Count);
        Assert.True(beforeFood.ATK < afterFood.ATK);
        Assert.True(beforeFood.Level < afterFood.Level);
    }

    [Fact]
    public void LevelUpAction_Fail_NoMsg()
    {
        var stateDelta = CreatePresetStateDelta(hasMsg: false);

        var food = DeriveRootStateFromAccountStateDelta(stateDelta)
            .InventoryState
            .RefrigeratorStateList[0];

        var action = new LevelUpAction(food.StateID);

        Assert.Throws<NotHaveRequiredException>(() =>
        {
            action.Execute(
                new DummyActionContext
                {
                    PreviousStates = stateDelta,
                    Signer = SignerAddress(),
                    Random = random,
                    Rehearsal = false,
                    BlockIndex = 1,
                }
            );
        });
    }

    [Fact]
    public void LevelUpAction_Fail_MaxLevel()
    {
        var stateDelta = CreatePresetStateDelta();

        var rootState = DeriveRootStateFromAccountStateDelta(stateDelta);

        var food = rootState.InventoryState.RefrigeratorStateList[0];

        var action = new LevelUpAction(food.StateID);

        Assert.Throws<AlreadyMaxLevelException>(() =>
            {
                action.Execute(
                    new DummyActionContext
                    {
                        PreviousStates = stateDelta.SetState(SignerAddress(), rootState.Serialize()),
                        Signer = SignerAddress(),
                        Random = random,
                        Rehearsal = false,
                        BlockIndex = 1,
                    }
                );
            }
        );
    }

    private IAccountStateDelta CreatePresetStateDelta(bool hasMsg = true)
    {
        IAccountStateDelta state = new DummyState();
        Address signerAddress = SignerAddress();

        var rootStateEncoded = state.GetState(signerAddress);
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : new RootState();

        InventoryState inventoryState = rootState.InventoryState;

        if (hasMsg)
        {
            for (int i = 0; i < 10000; i++)
            {
                inventoryState = inventoryState.AddItem(new ItemState(Guid.NewGuid(), MsgItemId));
            }
        }

        var food = RefrigeratorState.CreateFood(
            Guid.NewGuid(),
            1,
            "D",
            1,
            1,
            1,
            1,
            1,
            ImmutableList<Guid>.Empty
        );
        inventoryState = inventoryState.AddRefrigeratorItem(food);

        rootState.SetInventoryState(inventoryState);

        return state.SetState(signerAddress, rootState.Serialize());
    }
}
