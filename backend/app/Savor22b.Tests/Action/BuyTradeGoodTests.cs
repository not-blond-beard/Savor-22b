namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.Assets;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.States;
using Savor22b.States.Trade;
using Xunit;

public class BuyTradeGoodTests : ActionTests
{
    public BuyTradeGoodTests() { }

    [Fact]
    public void Execute_Success()
    {
        var (stateDelta, productId) = CreatePresetStateDelta();
        stateDelta = stateDelta.MintAsset(
            SignerAddress(),
            FungibleAssetValue.Parse(Currencies.KeyCurrency, "10")
        );

        var action = new BuyTradeGood(
            productId
        );

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

        var afterTradeInventoryState = DeriveTradeInventoryStateDelta(stateDelta);
        var afterRootState = DeriveRootStateFromAccountStateDelta(stateDelta);

        Assert.Empty(afterTradeInventoryState.TradeGoods);
        Assert.True(afterRootState.InventoryState.RefrigeratorStateList.Count == 1);
    }

    [Fact]
    public void Execute_Fail_InsufficientBalance()
    {
        var (stateDelta, productId) = CreatePresetStateDelta();

        var action = new BuyTradeGood(
            productId
        );

        Assert.Throws<InsufficientBalanceException>(() =>
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

    private (IAccountStateDelta, Guid) CreatePresetStateDelta()
    {
        IAccountStateDelta state = new DummyState();
        Address signerAddress = SignerAddress();

        var rootStateEncoded = state.GetState(signerAddress);
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : new RootState();
        TradeInventoryState tradeInventoryState = state.GetState(TradeInventoryState.StateAddress) is Bencodex.Types.Dictionary tradeInventoryStateEncoded
            ? new TradeInventoryState(tradeInventoryStateEncoded)
            : new TradeInventoryState();

        var foodProductId = Guid.NewGuid();

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

        tradeInventoryState = tradeInventoryState.RegisterGood(
            new FoodGoodState(
                signerAddress,
                foodProductId,
                FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"),
                food)
        );
        state = state.SetState(TradeInventoryState.StateAddress, tradeInventoryState.Serialize());
        return (state.SetState(signerAddress, rootState.Serialize()), foodProductId);
    }
}
