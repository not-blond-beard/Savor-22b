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

public class RegisterTradeGoodActionTests : ActionTests
{
    private const int LifeStoneItemId = 3;

    public RegisterTradeGoodActionTests() { }

    [Fact]
    public void RegisterTradeGoodActionExecute_Success_Food()
    {

        var stateDelta = CreatePresetStateDelta();
        var beforeState = DeriveRootStateFromAccountStateDelta(stateDelta);

        var beforeFood = beforeState.InventoryState.RefrigeratorStateList[0];

        var action = new RegisterTradeGoodAction(
            nameof(FoodGoodState),
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "10"
            ),
            beforeFood
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

        var afterState = DeriveRootStateFromAccountStateDelta(stateDelta);

        var afterTradeInventoryStateEncoded = stateDelta.GetState(SignerAddress());
        TradeInventoryState afterTradeInventoryState = afterTradeInventoryStateEncoded is Bencodex.Types.Dictionary bdict
            ? new TradeInventoryState(bdict)
            : throw new Exception();

        Assert.Empty(afterState.InventoryState.RefrigeratorStateList);

        var tradeGood = afterTradeInventoryState.TradeGoods.First(item => item.Value.SellerAddress == SignerAddress()).Value;

        if (tradeGood is FoodGoodState foodGoodState)
        {
            Assert.Equal(foodGoodState.Price, FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"));
            Assert.Equal(foodGoodState.Food?.StateID, beforeFood.StateID);
        }
        else
        {
            throw new Exception();
        }
    }

    // [Fact]
    // public void RegisterTradeGoodActionExecute_Success_Items()
    // {

    //     var stateDelta = CreatePresetStateDelta();
    //     var beforeState = DeriveRootStateFromAccountStateDelta(stateDelta);

    //     var beforeItems = beforeState.InventoryState.ItemStateList;

    //     var action = new RegisterTradeGoodAction(beforeItems.Select(i => i.StateID).ToList(), 10);

    //     stateDelta = action.Execute(
    //         new DummyActionContext
    //         {
    //             PreviousStates = stateDelta,
    //             Signer = SignerAddress(),
    //             Random = random,
    //             Rehearsal = false,
    //             BlockIndex = 1,
    //         }
    //     );

    //     var afterState = DeriveRootStateFromAccountStateDelta(stateDelta);
    //     var afterTradeStoreStateEncoded = stateDelta.GetState(SignerAddress());
    //     TradeStoreState afterTradeStoreState = afterTradeStoreStateEncoded is Bencodex.Types.Dictionary bdict
    //         ? new TradeStoreState(bdict)
    //         : throw new Exception();

    //     Assert.Empty(afterState.InventoryState.ItemStateList);
    //     Assert.Contains(afterTradeStoreState.ItemList, item => item.Item1 == beforeItems.Select(i => i.StateID).ToList());
    //     Assert.Equal(afterTradeStoreState.ItemList.First(item => item.Item1 == beforeItems.Select(i => i.StateID).ToList()).Item2, FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"));
    // }

    private IAccountStateDelta CreatePresetStateDelta()
    {
        IAccountStateDelta state = new DummyState();
        Address signerAddress = SignerAddress();

        var rootStateEncoded = state.GetState(signerAddress);
        RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
            ? new RootState(bdict)
            : new RootState();

        InventoryState inventoryState = rootState.InventoryState;

        inventoryState = inventoryState.AddItem(new ItemState(Guid.NewGuid(), LifeStoneItemId));
        inventoryState = inventoryState.AddItem(new ItemState(Guid.NewGuid(), LifeStoneItemId));

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
        var ingredient = RefrigeratorState.CreateIngredient(
            Guid.NewGuid(),
            1,
            "D",
            1,
            1,
            1,
            1
        );
        inventoryState = inventoryState.AddRefrigeratorItem(food);
        inventoryState = inventoryState.AddRefrigeratorItem(ingredient);

        rootState.SetInventoryState(inventoryState);

        return state.SetState(signerAddress, rootState.Serialize());
    }
}
