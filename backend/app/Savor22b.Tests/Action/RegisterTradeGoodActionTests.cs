namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.Assets;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.Model;
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
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "10"
            ),
            beforeFood.StateID
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
        var afterTradeInventoryState = DeriveTradeInventoryStateDelta(stateDelta);

        Assert.True(afterState.InventoryState.RefrigeratorStateList.Count == 1);

        var tradeGood = afterTradeInventoryState.TradeGoods.First(g => g.Value.SellerAddress == SignerAddress()).Value;

        if (tradeGood is FoodGoodState foodGoodState)
        {
            Assert.Equal(foodGoodState.Price, FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"));
            Assert.Equal(foodGoodState.Food.StateID, beforeFood.StateID);
        }
        else
        {
            throw new Exception();
        }
    }

    [Fact]
    public void RegisterTradeGoodActionExecute_Success_Items()
    {
        var stateDelta = CreatePresetStateDelta();
        var beforeState = DeriveRootStateFromAccountStateDelta(stateDelta);

        var beforeItems = beforeState.InventoryState.ItemStateList;

        var action = new RegisterTradeGoodAction(
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "10"
            ),
            beforeItems.Select(i => i.StateID).ToImmutableList()
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
        var afterTradeInventoryState = DeriveTradeInventoryStateDelta(stateDelta);

        Assert.Empty(afterState.InventoryState.ItemStateList);

        var tradeGood = afterTradeInventoryState.TradeGoods.First(g => g.Value.SellerAddress == SignerAddress()).Value;

        if (tradeGood is ItemsGoodState itemsGoodState)
        {
            Assert.Equal(itemsGoodState.Price, FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"));
            Assert.Equal(itemsGoodState.Items[0].StateID, beforeItems[0].StateID);
        }
        else
        {
            throw new Exception();
        }
    }

    [Fact]
    public void RegisterTradeGoodActionExecute_Success_DungeonConquest()
    {
        var stateDelta = CreatePresetStateDelta();
        var beforeState = DeriveRootStateFromAccountStateDelta(stateDelta);

        var dungeonId = beforeState.UserDungeonState.DungeonConquestHistories[0].DungeonId;

        var action = new RegisterTradeGoodAction(
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "10"
            ),
            dungeonId
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

        var afterRootState = DeriveRootStateFromAccountStateDelta(stateDelta);
        var afterTradeInventoryState = DeriveTradeInventoryStateDelta(stateDelta);
        var afterGlobalDungeonState = DeriveGlobalDungeonStateDelta(stateDelta);

        Assert.False(afterGlobalDungeonState.IsDungeonConquestAddress(dungeonId, SignerAddress()));

        var tradeGood = afterTradeInventoryState.TradeGoods.First(g => g.Value.SellerAddress == SignerAddress()).Value;

        if (tradeGood is DungeonConquestGoodState dungeonConquestGoodState)
        {
            Assert.Equal(dungeonConquestGoodState.Price, FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"));
            Assert.Equal(dungeonConquestGoodState.DungeonId, dungeonId);
        }
        else
        {
            throw new Exception();
        }
    }

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

        rootState.SetUserDungeonState(
            rootState.UserDungeonState.AddDungeonConquestHistory(
                new DungeonConquestHistoryState(1, 1, 1)
            )
        );
        var globalDungeonState = new GlobalDungeonState();
        globalDungeonState = globalDungeonState.SetDungeonConquestAddress(1, SignerAddress());

        state = state.SetState(GlobalDungeonState.StateAddress, globalDungeonState.Serialize());
        return state.SetState(signerAddress, rootState.Serialize());
    }
}
