namespace Savor22b.Tests.Action;

using System;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.Assets;
using Libplanet.State;
using Savor22b.Action;
using Savor22b.Action.Exceptions;
using Savor22b.States;
using Xunit;

public class ListItemInTradeStoreActionTests : ActionTests
{
    private static readonly int LifeStoneItemId = 3;

    public ListItemInTradeStoreActionTests() { }

    [Fact]
    public void ListItemInTradeStoreActionExecute_Success_Food()
    {

        var stateDelta = CreatePresetStateDelta();
        var beforeState = DeriveRootStateFromAccountStateDelta(stateDelta);

        var beforeFood = beforeState.InventoryState.RefrigeratorStateList[0];

        var action = new ListItemInTradeStoreAction(
            Guid.NewGuid(),
            beforeFood,
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "10"
            ));

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

        var afterTradeStoreStateEncoded = stateDelta.GetState(SignerAddress());
        TradeStoreState afterTradeStoreState = afterTradeStoreStateEncoded is Bencodex.Types.Dictionary bdict
            ? new TradeStoreState(bdict)
            : throw new Exception();

        Assert.Empty(afterState.InventoryState.RefrigeratorStateList);
        Assert.Equal(afterTradeStoreState.TradItems.First(item => item.Value.SellerAddress == SignerAddress()).Value.Price, FungibleAssetValue.Parse(Currencies.KeyCurrency, "10"));
        Assert.Contains(afterTradeStoreState.TradItems, item => item.Value.Food?.StateID == beforeFood.StateID);
    }

    // [Fact]
    // public void ListItemInTradeStoreActionExecute_Success_Items()
    // {

    //     var stateDelta = CreatePresetStateDelta();
    //     var beforeState = DeriveRootStateFromAccountStateDelta(stateDelta);

    //     var beforeItems = beforeState.InventoryState.ItemStateList;

    //     var action = new ListItemInTradeStoreAction(beforeItems.Select(i => i.StateID).ToList(), 10);

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
