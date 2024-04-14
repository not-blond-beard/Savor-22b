namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.States;
using Savor22b.States.Trade;
using Savor22b.Action.Exceptions;

[ActionType(nameof(BuyTradeGoodAction))]
public class BuyTradeGoodAction : SVRAction
{
    public BuyTradeGoodAction() { }

    public BuyTradeGoodAction(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId;

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(ProductId)] = ProductId.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        ProductId = plainValue[nameof(ProductId)].ToGuid();
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

        var good = tradeInventoryState.TradeGoods.First(g => g.Key == ProductId);

        states = states.TransferAsset(
            ctx.Signer,
            good.Value.SellerAddress,
            good.Value.Price,
            allowNegativeBalance: false
        );

        tradeInventoryState.TradeGoods.Remove(good.Key);

        switch (good.Value)
        {
            case FoodGoodState foodGoodState:
                inventoryState = inventoryState.AddRefrigeratorItem(foodGoodState.Food);
                break;
            case ItemsGoodState itemsGoodState:
                foreach (var itemState in itemsGoodState.Items)
                {
                    inventoryState = inventoryState.AddItem(itemState);
                }

                break;
            default:
                throw new InvalidValueException("Food or Items required");
        }

        rootState.SetInventoryState(inventoryState);
        states = states.SetState(TradeInventoryState.StateAddress, tradeInventoryState.Serialize());
        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
