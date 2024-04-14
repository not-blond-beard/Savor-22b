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

public class UpdateTradeGoodActionTests : ActionTests
{

    public UpdateTradeGoodActionTests() { }

    [Fact]
    public void RegisterTradeGoodActionExecute_Success()
    {

        var (beforeState, foodProductId) = CreatePresetStateDelta();
        var action = new UpdateTradeGoodAction(
            foodProductId,
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "10000"
            )
        );

        var afterState = action.Execute(
            new DummyActionContext
            {
                PreviousStates = beforeState,
                Signer = SignerAddress(),
                Random = random,
                Rehearsal = false,
                BlockIndex = 1,
            }
        );

        var afterTradeInventoryState = DeriveTradeInventoryStateDelta(afterState);
        var tradeGood = afterTradeInventoryState.TradeGoods.First(g => g.Value.SellerAddress == SignerAddress()).Value;

        if (tradeGood is FoodGoodState foodGoodState)
        {
            Assert.Equal(foodGoodState.Price, FungibleAssetValue.Parse(Currencies.KeyCurrency, "10000"));
        }
        else
        {
            throw new Exception();
        }
    }

    private (IAccountStateDelta, Guid) CreatePresetStateDelta()
    {
        IAccountStateDelta state = new DummyState();
        Address signerAddress = SignerAddress();

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

        return (state.SetState(TradeInventoryState.StateAddress, tradeInventoryState.Serialize()), foodProductId);
    }
}
