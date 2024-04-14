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

public class CancelRegisteredTradeGoodActionTests : ActionTests
{
    public CancelRegisteredTradeGoodActionTests() { }

    [Fact]
    public void Execute_Success_Food()
    {
        var (stateDelta, productId, _) = CreatePresetStateDelta();

        var action = new CancelRegisteredTradeGoodAction(
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

        Assert.True(afterTradeInventoryState.TradeGoods.Count == 1);
        Assert.True(afterRootState.InventoryState.RefrigeratorStateList.Count == 1);
    }

    [Fact]
    public void Execute_Success_Items()
    {
        var (stateDelta, _, productId) = CreatePresetStateDelta();

        var action = new CancelRegisteredTradeGoodAction(
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

        Assert.True(afterTradeInventoryState.TradeGoods.Count == 1);
        Assert.True(afterRootState.InventoryState.ItemStateList.Count == 1);
    }

    private (IAccountStateDelta, Guid, Guid) CreatePresetStateDelta()
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
        var itemsProductId = Guid.NewGuid();

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

        var item = new ItemState(Guid.NewGuid(), 1);

        tradeInventoryState = tradeInventoryState.RegisterGood(
            new FoodGoodState(
                signerAddress,
                foodProductId,
                FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"),
                food)
        );
        tradeInventoryState = tradeInventoryState.RegisterGood(
            new ItemsGoodState(
                signerAddress,
                itemsProductId,
                FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"),
                ImmutableList<ItemState>.Empty.Add(item))
        );

        state = state.SetState(TradeInventoryState.StateAddress, tradeInventoryState.Serialize());
        return (state.SetState(signerAddress, rootState.Serialize()), foodProductId, itemsProductId);
    }
}
