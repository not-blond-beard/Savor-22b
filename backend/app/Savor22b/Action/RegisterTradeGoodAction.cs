namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.States;
using Savor22b.States.Trade;
using Savor22b.Action.Exceptions;

[ActionType(nameof(RegisterTradeGoodAction))]
public class RegisterTradeGoodAction : SVRAction
{
    public RegisterTradeGoodAction() { }

    public RegisterTradeGoodAction(string goodType, FungibleAssetValue price, RefrigeratorState foodState)
    {
        GoodType = goodType;
        Price = price;
        FoodState = foodState;
        ItemStates = null;
    }

    public RegisterTradeGoodAction(string goodType, FungibleAssetValue price, ImmutableList<ItemState> itemStates)
    {
        GoodType = goodType;
        Price = price;
        FoodState = null;
        ItemStates = itemStates;
    }


    public string GoodType;

    public FungibleAssetValue Price;

    public RefrigeratorState? FoodState;

    public ImmutableList<ItemState>? ItemStates;

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(GoodType)] = GoodType.Serialize(),
            [nameof(Price)] = Price.ToBencodex(),
            [nameof(FoodState)] = FoodState is null ? Null.Value : FoodState.Serialize(),
            [nameof(ItemStates)] = ItemStates is null ? Null.Value : (Bencodex.Types.List)ItemStates.Select(i => i.Serialize()),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        GoodType = plainValue[nameof(GoodType)].ToString();
        Price = plainValue[nameof(Price)].ToFungibleAssetValue();
        FoodState = plainValue[nameof(FoodState)] is Null ? null : new RefrigeratorState((Dictionary)plainValue[nameof(FoodState)]);
        ItemStates = plainValue[nameof(ItemStates)] is Null ? null : ((List)plainValue[nameof(ItemStates)]).Select(e => new ItemState((Dictionary)e)).ToImmutableList();
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;

        RootState rootState = states.GetState(ctx.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();
        TradeInventoryState tradeInventoryState = states.GetState(TradeInventoryState.StateAddress) is Dictionary tradeInventoryStateEncoded
            ? new TradeInventoryState(tradeInventoryStateEncoded)
            : new TradeInventoryState();

        var inventoryState = rootState.InventoryState;

        switch (GoodType)
        {
            case nameof(FoodGoodState):
                if (FoodState is null)
                {
                    throw new InvalidValueException($"FoodState required");
                }

                inventoryState = inventoryState.RemoveRefrigeratorItem(FoodState.StateID);
                var foodGoodState = new FoodGoodState(ctx.Signer, ctx.Random.GenerateRandomGuid(), Price, FoodState);
                tradeInventoryState = tradeInventoryState.RegisterGood(foodGoodState);
                break;
            case nameof(ItemsGoodState):
                if (ItemStates is null)
                {
                    throw new InvalidValueException($"ItemStates required");
                }

                foreach (var item in ItemStates)
                {
                    inventoryState = inventoryState.RemoveItem(item.StateID);
                }
                var itemsGoodState = new ItemsGoodState(ctx.Signer, ctx.Random.GenerateRandomGuid(), Price, ItemStates);
                tradeInventoryState = tradeInventoryState.RegisterGood(itemsGoodState);
                break;
            default:
                throw new ArgumentException($"Unsupported TradeGood type: {GoodType}");
        }

        rootState.SetInventoryState(inventoryState);
        states = states.SetState(TradeInventoryState.StateAddress, tradeInventoryState.Serialize());
        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
